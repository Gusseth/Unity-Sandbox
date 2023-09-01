using Unity.Mathematics;
using UnityEngine;

// TODO: Implement directional blocking
public abstract class AbstractDirectionalBlocker : AbstractBlocker, IDirectionalHitter
{
    [SerializeField] float lookThreshold = 0.0125f;
    [SerializeField] BasicHitDirection lastBlockDirection;

    public override void PreBlock(float3 direction, AbstractActorBase actor)
    {
        base.PreBlock(direction, actor);
        BasicHitDirection attackDirection = GetAttackDirection(direction);
    }

    public void UpdateDirectionalIndicator(float3 deltaVelocity, IAttackDirectionalUI indicator)
    {
        indicator.UpdateTarget(FilterThreshold(deltaVelocity, BasicHitDirection.None));
    }

    protected BasicHitDirection GetAttackDirection(float3 direction)
    {
        BasicHitDirection blockDirection = FilterThreshold(direction, lastBlockDirection);
        lastBlockDirection = blockDirection;
        return blockDirection;
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