using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class MagicalBallHitterBox : AbstractHitterBox
{
    [SerializeField] SphereCollider actualCollider;
    [SerializeField] float forceStrength = 0;
    float3 lastPosition;
    float radius;
    bool hasHit;

    protected override void CastHitterBox(out float3 origin, out int nHits)
    {
        origin = lastPosition;
        float3 newPos = transform.position;
        float3 direction = newPos - origin;
        float length = math.length(direction);

        nHits = Physics.SphereCastNonAlloc(origin, radius, math.normalize(direction), hitBuffer, length, layerMask);
        lastPosition = newPos;
    }

    protected override void ProcessHits(in float3 origin, in int nHits)
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
                    OnBlockerHit(data);
                    break;
                }
                else if (hitLayerObject is IHurtBox hurtBox)
                {
                    if (!hurtBox.Active) continue;

                    Hit data = new(point, normal, this, hurtBox);

                    OnHurtBoxHit(data);
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

    protected override void OnBlockerHit(Block data)
    {
        if (data.blocker.CheckHit(data))
        {
            data.attacker.Hitter.Response(data);
            data.blocker.Response(data);
        }
    }

    protected override void OnHurtBoxHit(Hit data)
    {
        if (data.hurtBox.CheckHit(data))
        {
            data.hitterBox.Hitter.Response(data);
            data.hurtBox.HurtResponder.Response(data);
        }
    }

    public override void PreAttack(IHitter hitter, HitBoxLayer hitBoxLayer)
    {
        base.PreAttack(hitter, hitBoxLayer);
        lastPosition = transform.position;
        radius = actualCollider.radius * math.cmax(math.abs(transform.lossyScale));
    }

    protected override void OnHitterBoxHit(HitData hitData)
    {
        return;
    }

    protected override void Awake()
    {
        base.Awake();
        preventMultipleHits = true;
    }
}
