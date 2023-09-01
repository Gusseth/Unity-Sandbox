using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class AbstractHitter : MonoBehaviour, IHitter, IGiveOwnerMetadata
{
    [SerializeField] protected int damage;
    [SerializeField] protected bool attacking;
    [SerializeField] protected HitBoxLayer layer;
    [SerializeField] protected IHitterBox hitterBox;
    [SerializeField] protected AbstractActorBase user;

    public virtual int Damage => damage;
    public virtual bool Attacking { get => attacking; set => attacking = value; }
    public virtual HitBoxLayer HitBoxLayer => layer;
    public virtual GameObject Owner => user ? user.gameObject : null;
    public virtual AbstractActorBase Actor => user;

    public virtual bool CheckHit(HitData data)
    {
        return true;
    }

    public virtual bool PreAttack(float3 direction, AbstractActorBase actor)
    {
        user = actor;
        Attacking = true;
        hitterBox.PreAttack(this);
        return true;
    }

    public virtual void PostAttack()
    {
        Attacking = false;
        hitterBox.PostAttack(this);
    }

    public virtual void Response(HitData data)
    {
        switch (data)
        {
            case Block block:
                ChooseBlockFunction(block); break;
            case Hit hit:
                OnHit(hit); break;
        }
    }

    protected virtual void ChooseBlockFunction(Block data)
    {
        if (data.parry)
            OnParried(data);
        else
            OnBlocked(data);
    }

    public abstract void OnBlocked(Block data);
    public abstract void OnParried(Block data);
    public abstract void OnHit(Hit data);

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        hitterBox = GetComponent<IHitterBox>();
        hitterBox.Hitter = this;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (attacking) 
        {
            hitterBox.Attack();
        }
    }
}
