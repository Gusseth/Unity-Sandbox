using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Flags]
public enum InputFlags
{
    Started =       0b0001,
    Performed =     0b0010,
    Cancelled =     0b0100
}

[Serializable]
public record CastingData
{
    public Transform origin;
    public GameObject owner;
    public AbstractActorBase ownerActor;
    public GameObject target;

    public float3 point;
    public float3 distance;
    public float3 normal;

    public float speed;
    public float3 direction;
    public float3 acceleration;
    public Func<Transform, float3> directionFunction;
    public byte inputFlags;
}
