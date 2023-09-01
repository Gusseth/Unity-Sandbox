using Game.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public abstract class AbstractHitterBox : MonoBehaviour, IHitterBox
{
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected bool preventMultipleHits;
    protected IHitter hitter;
    protected HitBoxLayer hitBoxLayer;
    protected HitBoxFaction faction;
    protected ISet<Collider> alreadyHitColliders;
    protected RaycastHit[] hitBuffer;               // We buffer the sweep hits or else we generate garbage for GC to collect

    public IHitter Hitter { get => hitter; set => hitter = value; }

    public GameObject Owner => Hitter.Owner;

    public HitBoxLayer HitBoxLayer => hitBoxLayer;

    public virtual void Attack()
    {
        float3 origin;
        int nHits;
        CastHitterBox(out origin, out nHits);

        if (nHits > 1)
        {
            Algorithms.SortArray(ref hitBuffer, 0, nHits, Algorithms.RaycastHitDistanceComparer);
            ProcessHits(origin, nHits);
        }
    }

    protected abstract void CastHitterBox(out float3 origin, out int nHits);
    protected void CastHitterBox(out int nHits) { CastHitterBox(out _, out nHits); }

    protected abstract void ProcessHits(in float3 origin, in int nHits);

    protected abstract void OnBlockerHit(Block data);
    protected abstract void OnHurtBoxHit(Hit data);
    protected abstract void OnHitterBoxHit(HitData hitData);
 
    public virtual void PreAttack(IHitter hitter)
    {
        PreAttack(hitter, hitter.HitBoxLayer);
    }

    public virtual void PreAttack(IHitter hitter, HitBoxLayer hitBoxLayer)
    {
        this.hitter = hitter;
        this.hitBoxLayer = hitBoxLayer;
        if (preventMultipleHits)
        {
            alreadyHitColliders ??= new HashSet<Collider>();
        }
        faction = HitBoxFaction.Neutral;
        if (hitter.Actor != null)
        {
            faction = AbstractActorBase.ActorToHitBoxFaction(hitter.Actor.ActorFaction);
        }
    }

    public virtual void PostAttack(IHitter hitter)
    {
        if (preventMultipleHits)
        {
            alreadyHitColliders.Clear();
        }
    }

    protected virtual void Awake()
    {
        hitBuffer = new RaycastHit[Root.Constants.RaycastBufferSize];
    }
}
