using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothenNormals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter;
        if (TryGetComponent<MeshFilter>(out meshFilter))
        {
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;
            int[] tris = mesh.triangles;
            Vector3[] newNormals = new Vector3[vertices.Length];
            

        }
    }
}
