using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

[Serializable]
public abstract class GokuBase : ICastable, ICastableDFS, IHotbarDisplayable
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected GameObject worldModel;
    [SerializeField] protected string gokuName;
    [SerializeField] protected string gokuDescription;
    [SerializeField] protected int keCost = 0;
    [SerializeField] protected int initialDelay = 0;
    [SerializeField] protected int endDelay = 0;
    protected int accumulatedInitialDelay;
    protected int accumulatedEndDelay;
    protected bool casting;

    protected IMagicController prefabMagicController;

    public virtual string Name => gokuName;
    public virtual string Description => gokuDescription;
    public virtual int KeCost => keCost;
    public virtual int InitialDelay => initialDelay + accumulatedInitialDelay;
    public virtual int EndDelay => endDelay + accumulatedEndDelay;
    public virtual int ActualEndDelay => endDelay + accumulatedEndDelay;  // deprecated
    public virtual GameObject Model { get => prefab; set => prefab = value; }
    public virtual bool Casting => casting;
    public virtual string HotbarName => Name;
    public virtual string HotbarDescription => Description;
    public virtual GameObject WorldModel => worldModel;

    public abstract bool OnCast(CastingData castData, MonoBehaviour mono);

    public abstract UniTask<bool> OnCastStart(CastingData castData, ICastable parent, CancellationToken token);
    public abstract UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token);
    public abstract UniTask<bool> OnCastEnd(CastingData castData, ICastable parent, CancellationToken token);
    public virtual void OnOrder(IList orderedList, INorito parent, bool first = false, bool last = false)
    {
        if (first || last)
            SetDelays(parent, first);
        orderedList.Add(this);
        Initialize();
    }

    public virtual void OnEquipOut() { }

    protected virtual void SetDelays(INorito parent, bool first)
    {
        if (first)
        {
            accumulatedInitialDelay = parent.InitialDelay;
        }
        else
        {
            accumulatedEndDelay = parent.EndDelay;
        }
    }

    protected virtual void PreCast()
    {
        casting = true;
    }

    protected virtual void PostCast()
    {
        casting = false;
    }

    protected virtual void Initialize()
    {
        prefabMagicController ??= prefab.GetComponent<IMagicController>();
    }

    public virtual void CalculateKeCost()
    {
        Initialize();
        keCost = prefabMagicController.KeCost;
    }
}

[Serializable]
public class Goku : GokuBase
{
    bool castStartCalled;
    public override bool OnCast(CastingData castData, MonoBehaviour mono)
    {
        Debug.Log(Name);
        Initialize();
        if (prefabMagicController.CheckRequirements(castData))
        {
            GameObject casted = UnityEngine.Object.Instantiate(prefab);
            IMagicController magicController = casted.GetComponent<IMagicController>();
            magicController.Init(castData);
            return magicController.OnCast(castData);
        }
        return false;
    }

    public override async UniTask<bool> OnCastStart(CastingData castData, ICastable parent, CancellationToken token)
    {
        //Debug.Log(Name);
        castStartCalled = true;
        return prefabMagicController.OnCastStart(castData);
    }

    public override async UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        await UniTask.Delay(initialDelay, cancellationToken: token);
        Initialize();
        if (prefabMagicController.CheckRequirements(castData) && !token.IsCancellationRequested)
        {
            casting = true;
            IMagicController magicController = prefabMagicController.Instantiate(castData);
            if (magicController.OnCast(castData))
            {
                await UniTask.Delay(endDelay, cancellationToken: token);
                return true;
            }
        }
        casting = false;
        return false;
    }

    public override async UniTask<bool> OnCastEnd(CastingData castData, ICastable parent, CancellationToken token)
    {
        if (!castStartCalled)
        {
            // If the player holds the attack button at the end of a Norito,
            // the button-up input is registered as a button-down input so
            // the controls feel more fluid and responsive
            if (!await OnCastStart(castData, parent, token))
                return false;
        }
        return await OnCastAsync(castData, parent, token);
    }
}

[Serializable]
public class GokuHoldable : GokuBase, ICastableHoldable
{
    GameObject instance;
    IMagicController magicController;
    public override bool OnCast(CastingData castData, MonoBehaviour mono)
    {
        throw new NotImplementedException();
    }

    public override async UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        Initialize();
        if (prefabMagicController.CheckRequirements(castData) && !token.IsCancellationRequested)
        {
            casting = true;
            await UniTask.Delay(InitialDelay, cancellationToken: token);
            if (!token.IsCancellationRequested && prefabMagicController.CheckRequirements(castData))
            {
                if (parent == null || (parent != null && parent.Casting))
                {
                    magicController = prefabMagicController.Instantiate(castData);
                    if (magicController.OnCast(castData))
                    {
                        return true;
                    }
                }
            }
        }
        casting = false;
        return false;
    }

    public override async UniTask<bool> OnCastStart(CastingData castData, ICastable parent, CancellationToken token)
    {
        if (!prefabMagicController.OnCastStart(castData))
            return false;
        return await OnCastAsync(castData, parent, token);
    }

    public override async UniTask<bool> OnCastEnd(CastingData castData, ICastable parent, CancellationToken token)
    {
        if (magicController != null)
        {
            magicController.OnCastEnd();
            await UniTask.Delay(endDelay, cancellationToken: token);
            casting = false;
        }
        return true;
    }

    public override void OnEquipOut()
    {
        if (magicController != null)
        {
            magicController.OnEquipOut();
        }
    }
}

