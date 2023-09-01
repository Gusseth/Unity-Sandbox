using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class MagicalBallHitterBox : MonoBehaviour, IHitterBox
{
    [SerializeField] SphereCollider actualCollider;
    [SerializeField] LayerMask layerMask;
    [SerializeField] HitBoxLayer hitBoxLayer;
    [SerializeField] float forceStrength = 0;
    IHitter hitter;
    float3 lastPosition;
    HitBoxFaction faction;
    ISet<Collider> alreadyHitColliders; // don't need to clear because this will get destroyed anyways
    RaycastHit[] hitBuffer;             // We buffer the sweep hits or else we generate garbage for GC to collect
    float radius;
    bool hasHit;

    public IHitter Hitter { get => hitter; set => hitter = value; }

    public GameObject Owner => Hitter.Owner;

    public HitBoxLayer HitBoxLayer => hitBoxLayer;

    public void Attack()
    {
        float3 origin;
        int nHits;
        SweepHitterBox(out origin, out nHits);

        // Refactor later for flexibility. This only calls ProcessHits if we actually hit something else other than ourselves.
        if (nHits > 1)
        {
            //Array.Sort(hits, Algorithms.RaycastHitDistanceComparer);
            Algorithms.SortArray(ref hitBuffer, 0, nHits, Algorithms.RaycastHitDistanceComparer);
            ProcessHits(origin, nHits);
        }
    }

    private void SweepHitterBox(out float3 origin, out int nHits)
    {
        origin = lastPosition;
        float3 newPos = transform.position;
        float3 direction = newPos - origin;
        float length = math.length(direction);

        nHits = Physics.SphereCastNonAlloc(origin, radius, math.normalize(direction), hitBuffer, length, layerMask);
        lastPosition = newPos;
    }

    private void ProcessHits(float3 origin, in int nHits)
    {
        for (int i = 0; i < nHits; i++)
        {
            RaycastHit hit = hitBuffer[i];
            // We're detecting ourselves lol
            if (hit.collider == actualCollider) continue;
            if (alreadyHitColliders.Contains(hit.collider)) continue;

            alreadyHitColliders.Add(hit.collider);

            // I fucking hate Unity for making colliders that overlap in the first
            // sweep return a hit.point of (0, 0, 0) and a hit.normal of -direction.
            float3 point = hit.point;
            float3 normal = hit.normal;

            if (hit.point.Equals(float3.zero))
            {
                point = hit.collider.ClosestPoint(origin);
                normal = math.normalize(new float3(origin - point));
            }

            Root.Debug.DrawPointNormals(new Tuple<float3, float3>(point, normal));

            if (hit.collider.TryGetComponent(out IHitLayerObject hitLayerObject))
            {
                if (!MathHelpers.FlagContains((byte)HitBoxLayer, (byte)hitLayerObject.HitBoxLayer))
                    continue;
                if (Hitter.Actor != null && hitLayerObject.Owner == Hitter.Actor.gameObject) continue;

                if (hitLayerObject is IBlocker blocker && blocker.Blocking)
                {
                    hasHit = true;
                    Block data = new(point, normal, 0, this, blocker);
                    OnBlockerHit(blocker, data);
                    break;
                }
                else if (hitLayerObject is IHurtBox hurtBox)
                {
                    if (!hurtBox.Active) continue;

                    Hit data = new(point, normal, this, hurtBox);

                    OnHurtBoxHit(hurtBox, data);
                    hasHit = true;
                }

            }
            if (hit.collider.gameObject.layer != Root.Constants.HitboxLayerMaskIndex)
            {
                if (hit.collider.TryGetComponent(out Rigidbody rb))
                {
                    Vector3 explosiveForce = -normal * forceStrength;

                    rb.AddForceAtPosition(explosiveForce, point, ForceMode.Impulse);
                }

                hasHit = true;
            }

            alreadyHitColliders.Add(hit.collider);
        }

        if (hasHit)
            hitter.PostAttack();
    }

    private void OnBlockerHit(IBlocker blocker, Block data)
    {
        if (blocker.CheckHit(data))
        {
            data.attacker.Hitter.Response(data);
            data.blocker.Response(data);
        }
    }

    public void OnHurtBoxHit(IHurtBox hurtBox, HitData data)
    {
        Hit hitData = (Hit)data;
        if (hurtBox.CheckHit(hitData))
        {
            hitData.hitterBox.Hitter.Response(data);
            hitData.hurtBox.HurtResponder.Response(data);
        }
    }

    public void PreAttack(IHitter hitter)
    {
        lastPosition = transform.position;
        radius = actualCollider.radius * math.cmax(math.abs(transform.lossyScale));
        faction = HitBoxFaction.Neutral;
        if (hitter.Actor != null)
        {
            faction = AbstractActorBase.ActorToHitBoxFaction(hitter.Actor.ActorFaction);
        }
        alreadyHitColliders = new HashSet<Collider>();
    }

    public void PreAttack(IHitter hitter, AbstractActorBase actor)
    {
    }

    public void PostAttack(IHitter hitter)
    {
        
    }

    void Awake()
    {
        hitBuffer = new RaycastHit[Root.Constants.RaycastBufferSize];
    }
}
