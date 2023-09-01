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
    [SerializeField] static ConstantsInstance constants;
    static bool isPaused;

    public static Root Instance { get => root; }
    public static DebugInstance Debug { get => debug; }
    public static ConstantsInstance Constants { get => constants; }
    public static bool isGamePaused { get => isPaused; }

    void OnApplicationPause()
    {
        isPaused = true;
    }

    public class ConstantsInstance
    {
        int hitboxMaskIndex;
        uint raycastBufferSize = 32;    // Unity's engine limit is 128
        public int HitboxLayerMaskIndex { get => hitboxMaskIndex; }
        public uint RaycastBufferSize { get => raycastBufferSize; }

        public ConstantsInstance() {
            hitboxMaskIndex = LayerMask.NameToLayer("Hitbox");
        }

    }

    public sealed class DebugInstance
    {
        ICollection<Tuple<float3, float3>> hit_points_gizmo;
        ICollection<Tuple<float3, float3>> hit_points_gizmo_temp;
        ICollection<float3> lights;

        public DebugInstance()
        {
            hit_points_gizmo = new List<Tuple<float3, float3>>();
            hit_points_gizmo_temp = new List<Tuple<float3, float3>>();
            lights = new List<float3>();
        }

        public void DrawPointNormals(ICollection<Tuple<float3, float3>> point_normal_list)
        {
            hit_points_gizmo.AddRange(point_normal_list);
        }

        public void DrawPointNormals(Tuple<float3, float3> point_normal, bool persistent = true)
        {
            if (persistent)
                hit_points_gizmo.Add(point_normal);
            else
                hit_points_gizmo_temp.Add(point_normal);
        }

        public void DrawPointNormals(float3 point, float3 normal, bool persistent = true)
        {
            DrawPointNormals(new Tuple<float3, float3>(point, normal), persistent);
        }

        public void DrawLight(ICollection<float3> position_list)
        {
            lights.AddRange(position_list);
        }

        public void DrawLight(float3 lightPosition)
        {
            lights.Add(lightPosition);
        }



        private void DrawPointAndNormalGizmo(Tuple<float3, float3> pointNormal, Color color1, Color color2)
        {
            Gizmos.color = color1;
            Gizmos.DrawSphere(pointNormal.Item1, 0.025f);
            Gizmos.color = color2;
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
                DrawPointAndNormalGizmo(points, Color.red, Color.blue);
            }

            foreach (var points in hit_points_gizmo_temp)
            {
                DrawPointAndNormalGizmo(points, Color.yellow, Color.blue);
            }

            if (!Root.isPaused)
            {
                hit_points_gizmo_temp.Clear();
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
        constants = new ConstantsInstance();
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
