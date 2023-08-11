using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RayMovement : MonoBehaviour
{
    [SerializeField] public bool isMoving;
    [DoNotSerialize] public Vector3 velocity;
    [DoNotSerialize] public Vector3 acceleration;
    private Rigidbody rbody;

    // Start is called before the first frame update
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            velocity += acceleration * Time.deltaTime;
            rbody.velocity = velocity;
        }
    }
}
