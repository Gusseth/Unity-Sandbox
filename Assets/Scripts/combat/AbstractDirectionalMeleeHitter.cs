using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public abstract class AbstractDirectionalMeleeHitter : AbstractMeleeHitter, IDirectionalHitter
{
    [SerializeField] float lookThreshold = 0.0125f;
    BasicHitDirection lastAttackDirection = BasicHitDirection.Right;

    public override bool PreAttack(float3 direction, AbstractActorBase actor)
    {
        BasicHitDirection attackDirection = GetAttackDirection(direction);
        return base.PreAttack(direction, actor);
    }

    public void UpdateDirectionalIndicator(float3 deltaVelocity, IAttackDirectionalUI indicator)
    {
        indicator.UpdateTarget(FilterThreshold(deltaVelocity, BasicHitDirection.None));
    }

    protected BasicHitDirection GetAttackDirection(float3 direction)
    {
        BasicHitDirection attackDirection = FilterThreshold(direction, lastAttackDirection);
        lastAttackDirection = attackDirection;

        return attackDirection;
    }

    protected BasicHitDirection FilterThreshold(float3 direction, BasicHitDirection fallback)
    {
        if (math.length(direction) > lookThreshold)
        {
            Vector3 cardDirection = MathHelpers.CardinalizeDirection(direction);
            fallback = EnumHelpers.ToBasicHitDirection(cardDirection);
        }
        return fallback;
    }
}
