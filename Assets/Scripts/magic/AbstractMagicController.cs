using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMagicController
{
    public int KeCost { get; }
    public void Init();
    public bool OnCast(CastingData data);
    public bool CheckRequirements(CastingData data);
}

public abstract class AbstractMagicController : MonoBehaviour, IMagicController
{
    [SerializeField] int keCost;
    public int KeCost => keCost;

    public abstract void Init();

    public virtual bool OnCast(CastingData data)
    {
        gameObject.transform.position = data.origin.position;
        if (!data.ownerActor.KamiMode)
            data.ownerActor.AddKe(-keCost);
        return true;
    }

    public virtual bool CheckRequirements(CastingData data)
    {
        AbstractActorBase caster = data.ownerActor;
        return data.ownerActor.Ke >= keCost || caster.KamiMode;
    }
}
