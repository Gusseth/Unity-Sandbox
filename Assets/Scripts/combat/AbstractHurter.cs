using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstractHurter : MonoBehaviour, IHitResponder
{
    /// <summary>
    /// Length of time in milliseconds where the actor is immune to any form of damage after getting hit
    /// </summary>
    /// <remarks>The time waited is unscaled, therefore the actor will be damagable after exactly <c>iframesLength</c> ms</remarks>
    [SerializeField] protected int iframesLength = 500;
    [SerializeField] protected ActorBase actor;
    protected List<IHurtBox> hurtBoxes;
    protected bool iframesActive;

    public virtual bool CheckHit(HitData data)
    {
        if (iframesActive) return false;
        if (iframesLength > 0)
        {
            iframesActive = true;
            TimeHelpers.InvokeAsync(ResetIFrames, iframesLength, destroyCancellationToken, false);
        }
        return true;
    }

    public virtual void Response(HitData data)
    {
        switch (data)
        {
            case Hit hit:
                OnHurt(hit);
                break;
        }
    }

    protected virtual void OnHurt(Hit hit)
    {
        if (actor != null)
        {
            actor.AddDamage(hit);
        }
        Debug.Log($"You hit me, {gameObject.name}! My health is now {actor.Health}/{actor.MaxHealth}!");
    }

    protected virtual void Awake()
    {
        actor = GetComponent<ActorBase>();
        IHurtBox[] hurtBoxes = GetComponentsInChildren<IHurtBox>(includeInactive: true);
        this.hurtBoxes = new List<IHurtBox>(hurtBoxes);

        for (int i = 0; i < hurtBoxes.Length; i++)
        {
            hurtBoxes[i].HurtResponder = this;
        }
    }

    private void ResetIFrames()
    {
        iframesActive = false;
    }
}
