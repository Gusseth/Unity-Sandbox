using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetupEnvironment : MonoBehaviour
{
    [SerializeField] InputActionReference pause;

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 144;

        Cursor.lockState = CursorLockMode.Locked;
        pause.action.performed += OnPause;
        pause.action.Enable();
    }

    void OnPause(InputAction.CallbackContext context)
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
}
