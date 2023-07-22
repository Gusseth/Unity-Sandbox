using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class BetterCameraController : MonoBehaviour
{
    [SerializeField] float sensitivity = 5.0f;
    [SerializeField] InputActionReference look;
    [SerializeField] float3 acceleration;

    public float3 velocity { get; private set; }
    float3 rot;

    const float piOverTwo = math.PI / 2;
    const float twoPi = math.PI * 2;

    // Start is called before the first frame update
    void Start()
    {
        rot = velocity = float3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float3 input = new float3(math.radians(look.action.ReadValue<Vector2>()), 0);
        input.y *= -1;
        input *= sensitivity;

        velocity = math.lerp(velocity, input, acceleration * Time.deltaTime);

        rot += velocity.yxz * Time.deltaTime;
        rot.y %= twoPi;
        rot.x = math.clamp(rot.x, -piOverTwo, piOverTwo);

        transform.rotation = quaternion.Euler(rot);
    }
}
