using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class GenericMeleeHitter : MonoBehaviour, IMeleeHitter, IBlocker
{
    [SerializeField] bool isAttacking;
    [SerializeField] bool isBlocking;
    [SerializeField] bool isParrying;
    [SerializeField] bool isDirectional;
    [SerializeField] int rawDamage;
    [SerializeField] GameObject owner;
    [SerializeField] MeleeHitterBox hitterBox;
    [SerializeField] float lookThreshold = 0.0125f;
    [SerializeField] bool testMode;
    [SerializeField] HitBoxLayer hitBoxLayer;
    public int Damage { get => rawDamage; }
    public bool Blocking { get => isBlocking; set => isBlocking = value; }
    public bool Parry { get => isParrying; set => isParrying = value; }
    public bool Attacking { get => isAttacking; set => isAttacking = value; }
    public bool IsDirectional { get => isDirectional; set => isDirectional = value; }
    public GameObject Owner => owner;

    public HitBoxLayer HitBoxLayer => hitBoxLayer;

    BasicHitDirection lastAttackDirection = BasicHitDirection.Right;

    public bool CheckHit(HitData data)
    {
        return true;
    }

    public bool PreAttack(float3 direction, AbstractActorBase actor)
    {
        owner ??= actor.gameObject;
        BasicHitDirection attackDirection = GetAttackDirection(direction);
        Debug.Log($"Attacking from: {attackDirection}");
        Attacking = true;
        return true;
    }

    public void PostAttack()
    {
        Attacking = false;
        hitterBox.alreadyHitColliders.Clear();
    }

    public void PreBlock(float3 direction)
    {
        throw new System.NotImplementedException();
    }

    public void PostBlock()
    {
        throw new System.NotImplementedException();
    }

    public void Response(HitData data)
    {
        if (testMode && Attacking && data is Hit)
        {
            PostAttack();
            TimeHelpers.InvokeCoroutine(this, () => PreAttack(Vector3.zero, null), 1);
        }
    }

    BasicHitDirection GetAttackDirection(float3 direction)
    {
        BasicHitDirection attackDirection = FilterThreshold(direction, lastAttackDirection);
        lastAttackDirection = attackDirection;

        return attackDirection;
    }

    private BasicHitDirection FilterThreshold(float3 direction, BasicHitDirection fallback)
    {
        if (math.length(direction) > lookThreshold)
        {
            Vector3 cardDirection = MathHelpers.CardinalizeDirection(direction);
            fallback = EnumHelpers.ToBasicHitDirection(cardDirection);
        }
        return fallback;
    }

    // Start is called before the first frame update
    void Start()
    {
        hitterBox.Hitter = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {
            hitterBox.Attack();
        }
    }

    public void UpdateDirectionalIndicator(float3 direction, IAttackDirectionalUI indicator)
    {
        indicator.UpdateTarget(FilterThreshold(direction, BasicHitDirection.None));
    }

    public void OnBlock(Block data)
    {
        throw new System.NotImplementedException();
    }

    public void OnParry(Block data)
    {
        throw new System.NotImplementedException();
    }
}
