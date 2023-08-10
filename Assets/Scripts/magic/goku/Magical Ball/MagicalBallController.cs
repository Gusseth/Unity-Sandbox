using System.Collections;
using System.Collections.Generic;
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
        data.direction = data.directionFunction(data.origin);
        movement.velocity = data.direction * data.speed;
        movement.acceleration = data.acceleration;
        movement.isMoving = true;

        hitter.PreAttack(data.direction, data.ownerActor);
        return true;
    }

    // Start is called before the first frame update
    void Awake()
    {
        movement ??= GetComponent<RayMovement>();
        hitter ??= GetComponent<MagicalBallHitter>();
    }
}
