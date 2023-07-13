using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(PlayerInput))]
/*
* Simple noclip-like movement
*/
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] new GameObject camera;
    [SerializeField] GameObject subject;

    [SerializeField] float speed = 16.0f;
    [SerializeField] float jumpStrength = 10.0f;

    [SerializeField] bool gravityEnabled;
    [SerializeField] float g;
    [SerializeField] float gravityFactor = 1.0f;

    [SerializeField] InputActionReference move, jump;


    CharacterController character;
    public Vector3 movement { get; private set; }
    public Vector3 direction;
    float v_y;

    // Start is called before the first frame update
    void Awake()
    {
        camera = transform.GetChild(0).gameObject;
        subject = transform.gameObject;
        character = subject.GetComponent<CharacterController>();
        v_y = 0;
        g = Physics.gravity.y;
    }

    public void OnJump()
    {
        if (character.isGrounded)
        {
            v_y = jumpStrength;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Polling is more effective than waiting for a one-shot event to fire up
        direction = move.action.ReadValue<Vector2>().normalized;

        // Swap vec2.y into vec3.z since we don't want you going vertical
        (direction.z, direction.y) = (direction.y, 0);

        direction = subject.transform.TransformVector(direction).normalized;
        direction *= speed;

        if (!jump.action.WasPerformedThisFrame())
        {
            v_y = ApplyGravity(v_y);
        } 

        direction.y = v_y;
        movement = direction;
        character.Move(movement * Time.deltaTime);
    }

    float ApplyGravity(float velocity_y)
    {
        if (gravityEnabled) {
            if (character.isGrounded)
                return -1.0f;
            else
                return velocity_y + g * gravityFactor * Time.deltaTime;
        }
        else return velocity_y;
    }
}
