using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField] static Root root;
    [SerializeField] static DebugInstance debug;
    public static Root Instance { get => root; }
    public static DebugInstance Debug { get => debug; }

    public class DebugInstance
    {
        ICollection<Tuple<float3, float3>> hit_points_gizmo;
        ICollection<float3> lights;

        public DebugInstance()
        {
            hit_points_gizmo = new List<Tuple<float3, float3>>();
            lights = new List<float3>();
        }

        public void DrawPointNormals(ICollection<Tuple<float3, float3>> point_normal_list)
        {
            hit_points_gizmo.AddRange(point_normal_list);
        }

        public void DrawPointNormals(Tuple<float3, float3> point_normal)
        {
            hit_points_gizmo.Add(point_normal);
        }

        public void DrawLight(ICollection<float3> position_list)
        {
            lights.AddRange(position_list);
        }

        public void DrawLight(float3 lightPosition)
        {
            lights.Add(lightPosition);
        }

        private void DrawPointAndNormalGizmo(Tuple<float3, float3> pointNormal)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pointNormal.Item1, 0.025f);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(pointNormal.Item1, pointNormal.Item2);
        }

        private void DrawLightGizmo(float3 position)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(position, 0.025f);
        }

        public void Draw()
        {
            foreach (var points in hit_points_gizmo)
            {
                DrawPointAndNormalGizmo(points);
            }

            foreach (var light in lights)
            {
                DrawLightGizmo(light);
            }
        }

        public void Clear()
        {
            hit_points_gizmo.Clear();
        }
    }

    void Awake()
    {
        root = this;
        debug = new DebugInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        debug ??= new DebugInstance();
        Debug.Draw();
    }
}
