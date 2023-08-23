using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public interface IMagicController
{
    public int KeCost { get; }
    public int Timeout { get; }
    public GameObject gameObject { get; }   // expose Unity's MonoBehaviour.gameObject
    protected internal IMagicController Singleton { get; set; }
    public void Init();

    /// <summary>
    /// Controls how the a new isntance of this spell is casted. This is called in place of Instantiate() in the casting code.
    /// </summary>
    /// <remarks>
    /// This is called from the <u>singleton</u> (script in the prefab). Do not assume that this is run through any arbitrary instance!
    /// </remarks>
    /// <param name="data">Data concerning who, what, where, and how this spell is casted</param>
    /// <returns><c>IMagicController</c> component of the instantiated spell</returns>
    public IMagicController Instantiate(CastingData data);

    /// <summary>
    /// Called when the player has pressed-down the button to cast, before the actual instantiation of the object.
    /// </summary>
    /// <remarks>
    /// This is called from the <u>singleton</u> (script in the prefab). Do not assume that this is run through any arbitrary instance!
    /// </remarks>
    /// <param name="data">Data concerning who, what, where, and how this spell is casted</param>
    /// <returns><c>True</c> if all conditions are met, <c>False</c> otherwise</returns>
    public bool OnCastStart(CastingData data);

    /// <summary>
    /// Casts the spell. This is run immediately after the spell is instantiated.
    /// </summary>
    /// <remarks>
    /// This is run from the instantiated GameObject. Do whatever setup you wish.
    /// </remarks>
    /// <param name="data">Data concerning who, what, where, and how this spell is casted</param>
    /// <returns><c>True</c> if all conditions are met, <c>False</c> otherwise</returns>
    public bool OnCast(CastingData data);

    /// <summary>
    /// Called when the player has pressed-up the button to cast. In other words, after the cast button has been let go.
    /// </summary>
    /// <remarks>
    /// This is run from the instantiated GameObject.
    /// </remarks>
    /// <returns><c>True</c> if all conditions are met, <c>False</c> otherwise</returns>
    public bool OnCastEnd();

    /// <summary>
    /// Checks if the requirements have been met to cast the spell.
    /// </summary>
    /// <remarks>
    /// This is run from the instantiated GameObject.
    /// </remarks>
    /// <param name="data">Data concerning who, what, where, and how this spell is casted</param>
    /// <returns><c>True</c> if all conditions are met, <c>False</c> otherwise</returns>
    public bool CheckRequirements(CastingData data);

    /// <summary>
    /// Called if the player switches to a different equippable
    /// </summary>
    public void OnEquipOut();
}

public abstract class AbstractMagicController : MonoBehaviour, IMagicController
{
    [SerializeField] int keCost;
    [SerializeField] int timeoutInMilliseconds;
    protected IMagicController singleton;

    public int KeCost => keCost;
    public int Timeout => timeoutInMilliseconds;
    IMagicController IMagicController.Singleton { get => singleton; set => singleton = value; }

    public virtual void Init() { }

    public virtual IMagicController Instantiate(CastingData data)
    {
        IMagicController instance = Instantiate(gameObject).GetComponent<IMagicController>();
        PostInstantiate(instance, data);
        return instance;
    }

    public virtual bool OnCastStart(CastingData data)
    {
        return true;
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

    public virtual void OnEquipOut() { }

    protected virtual void PostInstantiate(IMagicController instance, CastingData data)
    {
        instance.Singleton = this;
        instance.Init();
    }
}

public abstract class AbstractHoldableMagicController : AbstractMagicController
{
    [SerializeField] protected bool active;
    protected IDamagableActor owner;

    public override bool OnCast(CastingData data)
    {
        owner = data.ownerActor;
        active = true;
        return true;
    }

    public override void OnEquipOut()
    {
        base.OnEquipOut();
        OnCastEnd();
    }
}
