using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class BetterCameraControllerBurst : MonoBehaviour, ICameraController
{
    [SerializeField] float sensitivity = 5.0f;
    [SerializeField] float deltaSensitivity = 0.125f;
    [SerializeField] InputActionReference look;
    [SerializeField] float3 acceleration;

    public float3 DeltaVelocity { get => deltaVelocity; }

    float3 deltaVelocity;
    NativeArray<float3> args;
    NativeArray<quaternion> quat;
    JobHandle jobHandle;
    float3 velocity;
    float3 rot;

    [BurstCompile]
    internal struct CameraControllerJob : IJob
    {
        public float deltaTime;
        public float2 rawInput;
        public float sensitivity;
        public float deltaSensitivity;
        public NativeArray<float3> args;
        public NativeArray<quaternion> quat;

        public void Execute()
        {
            float3 deltaVelocity = args[0];
            float3 velocity = args[1];
            float3 acceleration = args[2];
            float3 rot = args[3];

            float3 input = 0;
            input.xy = math.radians(rawInput);

            input.y *= -1;
            input *= sensitivity;

            velocity = math.lerp(velocity, input, acceleration * deltaTime);

            float3 correctedVelocity = velocity;
            correctedVelocity.y *= -1;

            deltaVelocity += correctedVelocity * deltaTime;
            deltaVelocity = math.lerp(deltaVelocity, float3.zero, acceleration * deltaTime * deltaSensitivity);

            rot += velocity.yxz * deltaTime;
            rot.y %= MathHelpers.twoPi;
            rot.x = math.clamp(rot.x, -MathHelpers.piOverTwo, MathHelpers.piOverTwo);

            args[0] = deltaVelocity;
            args[1] = velocity;
            args[2] = acceleration;
            args[3] = rot;
            quat[0] = quaternion.Euler(rot);
        }
    }

    private void OnEnable()
    {
        args = new NativeArray<float3>(4, Allocator.Persistent);
        quat = new NativeArray<quaternion>(1, Allocator.Persistent);
        rot = velocity = float3.zero;
        rot.y = MathHelpers.piOverTwo;
    }

    private void OnDisable()
    {
        if (args.IsCreated) args.Dispose();
        if (quat.IsCreated) quat.Dispose();
    }


    // Update is called once per frame
    void Update()
    {
        float2 rawInput = look.action.ReadValue<Vector2>();

        args[0] = deltaVelocity;
        args[1] = velocity;
        args[2] = acceleration;
        args[3] = rot;

        CameraControllerJob job = new CameraControllerJob
        {
            deltaTime = Time.deltaTime,
            rawInput = rawInput,
            sensitivity = sensitivity,
            deltaSensitivity = deltaSensitivity,
            args = args,
            quat = quat
        };

        jobHandle = job.Schedule();
    }

    void LateUpdate()
    {
        jobHandle.Complete();
        deltaVelocity = args[0];
        velocity = args[1];
        rot = args[3];

        transform.rotation = quat[0];
    }
}