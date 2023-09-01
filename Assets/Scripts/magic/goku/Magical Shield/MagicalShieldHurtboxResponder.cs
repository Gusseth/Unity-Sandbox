using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class MagicalShieldHurtboxResponder : AbstractBlocker
{
    [SerializeReference] protected BoxCollider shieldCollider;
    public override void OnBlock(Block data)
    {
        Debug.Log($"Blocked \"{data.attacker.Hitter.Name}\"");
    }

    public override void OnParry(Block data)
    {
        Debug.Log($"Parried \"{data.attacker.Hitter.Name}\"");
    }

    public void SetSize(float3 size)
    {
        shieldCollider.size = size;
    }

    protected override void Awake()
    {
        base.OnEnable();
        shieldCollider ??= GetComponent<BoxCollider>();
    }
}
