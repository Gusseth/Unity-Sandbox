using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMagicController
{
    public int KeCost { get; }
    public int Timeout { get; }
    public void Init();
    public bool OnCast(CastingData data);
    public bool CheckRequirements(CastingData data);
}

public abstract class AbstractMagicController : MonoBehaviour, IMagicController
{
    [SerializeField] int keCost;
    [SerializeField] int timeoutInMilliseconds;
    public int KeCost => keCost;
    public int Timeout => timeoutInMilliseconds;

    public abstract void Init();

    public virtual bool OnCast(CastingData data)
    {
        if (Timeout >= 0)
            TimeHelpers.InvokeAsync(DestroyCastable, Timeout, this.GetCancellationTokenOnDestroy());
        gameObject.transform.position = data.origin.position;
        if (!data.ownerActor.KamiMode)
            data.ownerActor.AddKe(-keCost);
        return true;
    }

    public virtual void DestroyCastable()
    {
        Destroy(gameObject);
    }

    public virtual bool CheckRequirements(CastingData data)
    {
        AbstractActorBase caster = data.ownerActor;
        return data.ownerActor.Ke != 0 || caster.KamiMode;
    }
}
