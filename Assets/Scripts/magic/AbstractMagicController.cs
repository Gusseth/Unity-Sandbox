using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMagicController
{
    public int KeCost { get; }
    public int Timeout { get; }
    public GameObject gameObject { get; }   // expose Unity's MonoBehaviour.gameObject
    protected internal IMagicController Singleton { get; set;  }
    public void Init();
    public IMagicController Instantiate(CastingData data);
    public bool OnCastStart(CastingData data);
    public bool OnCast(CastingData data);
    public bool OnCastEnd();
    public bool CheckRequirements(CastingData data);
}

public abstract class AbstractMagicController : MonoBehaviour, IMagicController
{
    [SerializeField] int keCost;
    [SerializeField] int timeoutInMilliseconds;
    protected IMagicController singleton;

    public int KeCost => keCost;
    public int Timeout => timeoutInMilliseconds;
    IMagicController IMagicController.Singleton { get => singleton; set => singleton = value; }

    public abstract void Init();

    public virtual IMagicController Instantiate(CastingData data)
    {
        IMagicController instance = Instantiate(gameObject).GetComponent<IMagicController>();
        PostInstantiate(instance, data);
        return instance;
    }

    public virtual bool OnCastStart(CastingData data)
    {
        return false;
    }

    public virtual bool OnCast(CastingData data)
    {
        if (Timeout >= 0)
            TimeHelpers.InvokeAsync(DestroyCastable, Timeout, this.GetCancellationTokenOnDestroy());
        gameObject.transform.position = data.origin.position;
        if (!data.ownerActor.KamiMode)
            data.ownerActor.AddKe(-keCost);
        return true;
    }

    public virtual bool OnCastEnd()
    {
        return false;
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

    protected virtual void PostInstantiate(IMagicController instance, CastingData data)
    {
        instance.Singleton = this;
        instance.Init();
    }
}
