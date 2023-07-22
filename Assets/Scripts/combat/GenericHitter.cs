using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHitter : MonoBehaviour, IMeleeHitter
{
    [SerializeField] bool isAttacking;
    [SerializeField] bool isBlocking;
    [SerializeField] bool isParrying;
    [SerializeField] int rawDamage;
    [SerializeField] HitterBox hitterBox;
    public int Damage { get => rawDamage; }
    public bool Blocking { get => isBlocking; set => isBlocking = value; }
    public bool Parry { get => isParrying; set => isParrying = value; }
    public bool Attacking { get => isAttacking; set => isAttacking = value; }

    public bool CheckHit(HitData data)
    {
        return true;
    }

    public void PreAttack()
    {
        throw new System.NotImplementedException();
    }

    public void PostAttack()
    {
        hitterBox.alreadyHitColliders.Clear();
    }

    public void PreBlock()
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
