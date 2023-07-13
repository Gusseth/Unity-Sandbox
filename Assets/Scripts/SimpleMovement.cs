using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Simple noclip-like movement
 */
public class SimpleMovement : MonoBehaviour
{
    [SerializeField] bool noclip;
    [SerializeField] new GameObject camera;
    [SerializeField] float speed = 16.0f;

    // Start is called before the first frame update
    void Start()
    {
        camera = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float X = Input.GetAxisRaw("Horizontal");
        float Z = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(X, 0, Z);
        direction = camera.transform.TransformVector(direction);

        if (!noclip)
            direction.y = 0;

        direction = direction.normalized;

        transform.Translate(direction * speed * Time.deltaTime);
    }
}
