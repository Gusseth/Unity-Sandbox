using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GenericHitter : MonoBehaviour, IMeleeHitter
{
    [SerializeField] bool isAttacking;
    [SerializeField] bool isBlocking;
    [SerializeField] bool isParrying;
    [SerializeField] int rawDamage;
    [SerializeField] HitterBox hitterBox;
    [SerializeField] float lookThreshold = 0.0125f;
    public int Damage { get => rawDamage; }
    public bool Blocking { get => isBlocking; set => isBlocking = value; }
    public bool Parry { get => isParrying; set => isParrying = value; }
    public bool Attacking { get => isAttacking; set => isAttacking = value; }

    BasicHitDirection lastAttackDirection = BasicHitDirection.Right;

    public bool CheckHit(HitData data)
    {
        return true;
    }

    public void PreAttack(float3 direction)
    {
        BasicHitDirection attackDirection = ProcessDirection(direction);
        Debug.Log($"Attacking from: {attackDirection}");
        Attacking = true;
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
        
    }

    BasicHitDirection ProcessDirection(float3 direction)
    {
        BasicHitDirection attackDirection = lastAttackDirection;
        float3 abs_direction = math.abs(direction);
        float highest = math.cmax(abs_direction);
        if (math.length(direction) > lookThreshold)
        {
            int i = 0;
            BasicHitDirection positive = 0, negative = 0;
            for (; i < 3; i++)
            {
                if (abs_direction[i] == highest)
                {
                    highest = direction[i];
                    break;
                }
            }
            switch (i)
            {
                case 0:
                    positive = BasicHitDirection.Right;
                    negative = BasicHitDirection.Left;
                    break;
                case 1:
                    positive = BasicHitDirection.Down;
                    negative = BasicHitDirection.Up;
                    break;
            }
            attackDirection = (BasicHitDirection)MathHelpers.FloatDirection(highest, positive, negative);
            lastAttackDirection = attackDirection;
        }
        return attackDirection;
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
}
