using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RayMovement : MonoBehaviour
{
    [SerializeField] public bool isMoving;
    [SerializeField] public int lifetimeInSeconds = 10;
    [DoNotSerialize] public Vector3 velocity;
    [DoNotSerialize] public Vector3 acceleration;
    private uint lifetimeLimit;
    private uint lifetime;
    private Rigidbody rbody;

    // Start is called before the first frame update
    void Awake()
    {
        uint fixedFramesPerSecond = (uint)(1.0f / Time.fixedDeltaTime);
        lifetimeLimit = (uint)(lifetimeInSeconds * fixedFramesPerSecond);
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

    private void FixedUpdate()
    {
        if (lifetimeInSeconds < 0)
            return;
        lifetime += 1;
        if (lifetime > lifetimeLimit)
        {
            Destroy(gameObject);
        }
    }
}
