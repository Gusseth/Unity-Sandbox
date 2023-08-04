using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class CastingData
{
    public Transform origin;
    public GameObject owner;
    public AbstractActorBase ownerActor;
    public GameObject target;
    public AbstractActorBase targetActor;
    public float speed;
    public float3 direction;
    public float3 acceleration;
    public Func<Transform, float3> directionFunction;
    public Hand hand;
}
