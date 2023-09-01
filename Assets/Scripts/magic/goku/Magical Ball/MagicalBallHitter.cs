using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MagicalBallHitter : AbstractHitter
{
    public IMagicController controller;
    public override string Name { get => controller.Name; set => _ = value; }

    public override void OnBlocked(Block data)
    {
        return;
    }

    public override void OnHit(Hit data)
    {
        return;
    }

    public override void OnParried(Block data)
    {
        return;
    }

    public override void PostAttack()
    {
        base.PostAttack();
        Destroy(gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        controller ??= GetComponent<IMagicController>();
    }
}
