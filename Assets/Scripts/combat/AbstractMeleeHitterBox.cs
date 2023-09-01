using Game.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public abstract class AbstractMeleeHitterBox : AbstractHitterBox
{ 
    [SerializeField] int verticalSubdivisions = 8;  // Works best with powers of 2
    [SerializeField] protected BoxCollider boxCollider;
    protected float thickness;
    protected float3 size;
    protected float3 halfExtents;
    protected float distance;

    public override void PreAttack(IHitter hitter, HitBoxLayer hitBoxLayer)
    {
        base.PreAttack(hitter, hitBoxLayer);
        size = boxCollider.size * (float3)transform.lossyScale;
        halfExtents = new float3(size.x, thickness, size.z) / 2;
        distance = size.y - thickness;
    }

    protected override void CastHitterBox(out float3 origin, out int nHits)
    {
        float3 direction = transform.up;
        float3 center = math.transform(transform.localToWorldMatrix, boxCollider.center);
        origin = center - direction * (distance / 2);

        nHits = Physics.BoxCastNonAlloc(origin, halfExtents, direction, hitBuffer, transform.rotation, distance, layerMask);
    }

    protected override void ProcessHits(in float3 origin, in int nHits)
    {
        for (int i = 0; i < nHits; i++)
        {
            RaycastHit hit = hitBuffer[i];

            Collider c = hit.collider;
            if (c == null ||
                c == boxCollider || // We're detecting ourselves lol
                (preventMultipleHits && alreadyHitColliders.Contains(c)))
                continue;

            if (preventMultipleHits)
            {
                alreadyHitColliders.Add(hit.collider);
            }

            hit = PreProcessHit(hit);
            Root.Debug.DrawPointNormals(hit.point, hit.normal);

            if (hit.collider.TryGetComponent(out IHitLayerObject hitLayerObject))
            {
                if (!MathHelpers.FlagContains((byte)HitBoxLayer, (byte)hitLayerObject.HitBoxLayer)) continue;
                if (hitLayerObject.Owner == Owner) continue;

                switch (hitLayerObject)
                {
                    case IBlocker blocker:
                        if (!OnBlockerGuard(hit, blocker)) continue;
                        OnBlockerHit(MakeBlockData(hit, blocker));
                        goto ProcessHits_Break;
                    case IHurtBox hurtBox:
                        if (!OnHurtBoxGuard(hit, hurtBox)) continue;
                        OnHurtBoxHit(MakeHurtData(hit, hurtBox));
                        break;
                    case IHitter hitterBox:
                        if (!OnHitterBoxGuard(hit, hitterBox)) continue;
                        OnHitterBoxHit(MakeHitterData(hit, hitterBox));
                        break;
                }
            }
        }
        ProcessHits_Break:;
    }

    private bool OnHitterBoxGuard(RaycastHit hit, IHitter hitterBox)
    {
        if (!hitterBox.Attacking) return false;
        return true;
    }

    private HitData MakeHitterData(RaycastHit hit, IHitter hitter)
    {
        throw new NotImplementedException();
    }

    private bool OnHurtBoxGuard(RaycastHit hit, IHurtBox hurtBox)
    {
        if (!hurtBox.Active) return false;
        return true;
    }

    protected Hit MakeHurtData(RaycastHit hit, IHurtBox hurtBox)
    {
        return new Hit(
            Hitter.Damage,
            hit.point,
            hit.normal,
            hurtBox,
            this
            );
    }

    protected virtual bool OnBlockerGuard(RaycastHit hit, IBlocker blocker)
    {
        if (!blocker.Blocking) return false;
        return true;
    }

    protected virtual RaycastHit PreProcessHit(RaycastHit hit)
    {
        if (hit.point.Equals(float3.zero))
        {
            hit.point = math.transform(transform.localToWorldMatrix, boxCollider.center);
        }
        return hit;
    }

    protected Block MakeBlockData(RaycastHit hit, IBlocker blocker)
    {
        return new Block(
            Hitter.Damage,
            hit.point,
            hit.normal,
            blocker.Parry,
            0,
            this,
            blocker
            );
    }

    protected override void OnBlockerHit(Block data)
    {
        data.blocker.Response(data);
        //data.attacker.Hitter.Response(data);
        Hitter.Response(data);
    }

    protected override void OnHitterBoxHit(HitData hitData) { }

    protected override void OnHurtBoxHit(Hit data)
    {
        if (data.hurtBox.CheckHit(data))
        {
            data.hitterBox.Hitter.Response(data);
            data.hurtBox.HurtResponder.Response(data);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        boxCollider ??= GetComponent<BoxCollider>();
        thickness = (boxCollider.size * (float3)transform.lossyScale).y / verticalSubdivisions;
    }
}