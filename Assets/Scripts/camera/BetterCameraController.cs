using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class BetterCameraController : MonoBehaviour, ICameraController
{
    [SerializeField] float sensitivity = 5.0f;
    [SerializeField] float deltaSensitivity = 0.125f;
    [SerializeField] InputActionReference look;
    [SerializeField] float3 acceleration;

    public float3 DeltaVelocity { get => deltaVelocity; }

    float3 deltaVelocity;
    float3 velocity;
    float3 rot;

    // Start is called before the first frame update
    void Start()
    {
        rot = velocity = float3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float2 rawInput = look.action.ReadValue<Vector2>();
        float3 input = 0;
        input.xy = math.radians(rawInput);

        input.y *= -1;
        input *= sensitivity;

        velocity = math.lerp(velocity, input, acceleration * Time.deltaTime);

        float3 correctedVelocity = velocity;
        correctedVelocity.y *= -1;

        deltaVelocity += correctedVelocity * Time.deltaTime;
        deltaVelocity = math.lerp(deltaVelocity, float3.zero, acceleration * Time.deltaTime * deltaSensitivity);

        rot += velocity.yxz * Time.deltaTime;
        rot.y %= MathHelpers.twoPi;
        rot.x = math.clamp(rot.x, -MathHelpers.piOverTwo, MathHelpers.piOverTwo);

        transform.rotation = quaternion.Euler(rot);
    }
}
