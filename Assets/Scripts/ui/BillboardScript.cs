using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float3 billboard = transform.position;
        float3 cam = mainCamera.transform.position;
        transform.rotation = mainCamera.transform.rotation;
    }
}
