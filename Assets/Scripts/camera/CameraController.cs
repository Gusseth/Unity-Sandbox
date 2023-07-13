using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float sensitivity = 5.0f;
    [SerializeField] bool smooth = false;
    [SerializeField] float snappiness = 1.0f;
    [SerializeField] float baseSpeed = 10.0f;

    [SerializeField] InputActionReference look;
    Quaternion q_rotation = Quaternion.identity;

    float rotX, rotY = 0;

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Polling is more effective than waiting for a one-shot event to fire up
        Vector3 input = look.action.ReadValue<Vector2>() * baseSpeed * sensitivity * Time.deltaTime;
        if (input != Vector3.zero)
        {
            // Nvm Quaternions win the day...
            rotY += input.x;
            rotY %= 360.0f; // Prevents loss of precision and
                            // lag by constraining float to [0, 360)

            rotX -= input.y;
            rotX = Mathf.Clamp(rotX, -90, 90);
            input = Vector3.zero;

            // Combine both rotations
            q_rotation = Quaternion.Euler(rotX, rotY, 0);

            if (!smooth)
            {
                transform.rotation = q_rotation;
                return;
            }               
        }
        if (smooth)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, q_rotation, snappiness * Time.deltaTime);
        }
    }
}
