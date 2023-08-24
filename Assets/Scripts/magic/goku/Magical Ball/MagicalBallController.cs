using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MagicalBallController : AbstractMagicController
{
    [SerializeField] RayMovement movement;
    [SerializeField] MagicalBallHitter hitter;

    public override void Init()
    {
        Awake();
    }

    public override bool OnCast(CastingData data)
    {
        base.OnCast(data);
        float3 direction;

        if (data.directionFunction != null)
        {
            direction = data.directionFunction(data.origin);
        } 
        else
        {
            direction = data.direction;
        }

        movement.velocity = direction * data.speed;
        movement.acceleration = data.acceleration;
        movement.isMoving = true;

        hitter.PreAttack(direction, data.ownerActor);
        return true;
    }


    // Start is called before the first frame update
    void Awake()
    {
        movement ??= GetComponent<RayMovement>();
        hitter ??= GetComponent<MagicalBallHitter>();
    }
}
