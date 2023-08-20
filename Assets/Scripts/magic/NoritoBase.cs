﻿using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Unity.Mathematics;

public interface ICastableData
{
    /// <summary>
    /// Displayed name of this castable
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Displayed description of this castable
    /// </summary>
    public string Description { get; }
    /// <summary>
    /// How much Ke would be expended after fully casting
    /// </summary>
    public int KeCost { get; }
    /// <summary>
    /// The time (in milliseconds) delayed BEFORE casting this
    /// </summary>
    public int InitialDelay { get; }
    /// <summary>
    /// The time (in milliseconds) delayed AFTER casting this
    /// </summary>
    public int EndDelay { get; }
    /// <summary>
    /// Legacy code when the first implementation baked Norito delay times inside the Goku
    /// </summary>
    [Obsolete]
    public int ActualEndDelay { get; }
    /// <summary>
    /// Returns True if the castable is in the middle of casting the spell
    /// </summary>
    public bool Casting { get; }
    /// <summary>
    /// The world model used by this castable
    /// </summary>
    public GameObject Model { get; set; }
    /// <summary>
    /// Recalculates the KeCost of this castable and all of its children
    /// </summary>
    public void CalculateKeCost();
}

public interface ICastable : ICastableData
{
    /// <summary>
    /// Casts this Goku or any of this Norito's children.  
    /// </summary>
    /// <remarks><u><b>DO NOT AWAIT IN THE MAIN THREAD!</b></u></remarks>
    /// <param name="castData">Data that is passed to the child</param>
    /// <param name="parent">The parent castable, if there is any</param>
    /// <param name="token">The cancellation token to watch</param>
    /// <returns><c>True</c> under normal operation, <c>False</c> if further execution must be stopped</returns>
    public UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token);
}

public interface ICastableChargable : ICastable
{
    public UniTask<bool> OnCastStart(CastingData castData, CancellationToken token);
    public UniTask<bool> OnCastEnd(CancellationToken token);
}

public interface INorito : ICastableData
{
    /// <summary>
    /// Is this Norito auto-casted? (Sequential if False)
    /// </summary>
    public bool AutoCast { get; set; }
    /// <summary>
    /// Has this Norito gone through all of its castables and is back to the first castable (i == 0)
    /// </summary>
    public bool CycleComplete { get; }
}

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

    public abstract UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token);

    public virtual void OnOrder(IList orderedList, INorito parent, bool first = false, bool last = false)
    {
        if (first || last)
            SetDelays(parent, first);
        orderedList.Add(this);
        Initialize();
    }

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
    public override bool OnCast(CastingData castData, MonoBehaviour mono)
    {
        Debug.Log(Name);
        Initialize();
        if (prefabMagicController.CheckRequirements(castData))
        {
            GameObject casted = UnityEngine.Object.Instantiate(prefab);
            IMagicController magicController = casted.GetComponent<IMagicController>();
            magicController.Init();
            return magicController.OnCast(castData);
        }
        return false;
    }

    public override async UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        if (!castData.inputContext.canceled) return false;
        await UniTask.Delay(initialDelay, cancellationToken: token);
        Debug.Log(Name);
        Initialize();
        if (prefabMagicController.CheckRequirements(castData) && !token.IsCancellationRequested)
        {
            casting = true;
            IMagicController magicController = prefabMagicController.Instantiate(castData);
            if (magicController.OnCast(castData))
            {
                await UniTask.Delay(endDelay, cancellationToken: token);
                casting = false;
                return true;
            }
        }
        return false;
    }
}

[Serializable]
public class GokuChargable : GokuBase, ICastableChargable
{
    GameObject instance;
    IMagicController magicController;
    public override bool OnCast(CastingData castData, MonoBehaviour mono)
    {
        throw new NotImplementedException();
    }

    public override async UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        if (!casting && castData.inputContext.started)
        {
            return await OnCastStart(castData, token);
        }
        else if (castData.inputContext.canceled)
        {
            return await OnCastEnd(token);
        }
        else 
            return false;
    }

    public async UniTask<bool> OnCastStart(CastingData castData, CancellationToken token)
    {
        Debug.Log(Name);
        Initialize();
        if (prefabMagicController.CheckRequirements(castData) && !token.IsCancellationRequested)
        {
            casting = true;

            await UniTask.Delay(InitialDelay, cancellationToken: token);

            GameObject casted = UnityEngine.Object.Instantiate(prefab, castData.owner.transform);
            magicController = casted.GetComponent<IMagicController>();
            magicController.Init();
            if (magicController.OnCastStart(castData))
            {
                return true;
            }
        }
        return false;
    }

    public async UniTask<bool> OnCastEnd(CancellationToken token)
    {
        magicController.OnCastEnd();
        await UniTask.Delay(endDelay, cancellationToken: token);
        casting = false;
        return true;
    }
}

/*
 * Issues with SubclassSelector? Download the package here:
 * https://github.com/mackysoft/Unity-SerializeReferenceExtensions
 */

[Serializable]
public abstract class NoritoBase : ICastable, IHotbarDisplayable
{
    [SerializeReference, SubclassSelector] public List<ICastable> castables = new List<ICastable>();
    [SerializeField] protected GameObject worldModelPrefab;
    [SerializeField] protected int totalKeCost = 0;
    [SerializeField] protected int initialDelay = 0;
    [SerializeField] protected int endDelay = 0;
    [SerializeField] protected string noritoName;
    [SerializeField] protected string noritoDescription;

    [SerializeField] protected bool casting = false;
    [SerializeField] protected int i;

    protected bool keAlreadyCalculated = false;
    protected bool finishedAtLeastOnce = false;

    public virtual string Name => noritoName;
    public virtual string Description => noritoDescription;
    public virtual int KeCost => totalKeCost;
    public virtual int InitialDelay => initialDelay;
    public virtual int EndDelay => endDelay;
    public virtual int ActualEndDelay => endDelay + castables.Last().EndDelay;
    public virtual GameObject Model { get => worldModelPrefab; set => worldModelPrefab = value; }
    public virtual bool Casting => casting;
    public virtual string HotbarName => Name;
    public virtual string HotbarDescription => Description;
    public virtual GameObject WorldModel => worldModelPrefab;

    public abstract UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token);

    protected void IncrementI()
    {
        if (i < castables.Count - 1)
            i++;
        else
        {
            i = 0;
            finishedAtLeastOnce = true;
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

    public virtual void CalculateKeCost()
    {
        totalKeCost = 0;
        foreach (ICastable castable in castables)
        {
            castable.CalculateKeCost();
            totalKeCost += castable.KeCost;
        }
        keAlreadyCalculated = true;
    }
}


[Serializable]
public class Norito : NoritoBase, INorito
{
    [SerializeField] bool autoCast = true;

    public bool AutoCast { get => autoCast; set => autoCast = value; }
    public bool CycleComplete => cycleComplete();
    private bool cycleComplete()
    {
        return i == 0 && finishedAtLeastOnce;
    }

    public override async UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        if (casting) return false;
        if (!castData.inputContext.canceled) return false;

        if (!keAlreadyCalculated)
            CalculateKeCost();

        PreCast();
        if (i == 0)
        {
            // Start of the norito
            await UniTask.Delay(InitialDelay);
        }

        bool result;

        if (autoCast)
            result = await OnAutoCastAsync(castData, this, token);
        else
            result = await OnSequentialCastAsync(castData, this, token);

        if (finishedAtLeastOnce)
        {
            // End of the norito
            await UniTask.Delay(EndDelay);
            finishedAtLeastOnce = false;
        }

        PostCast();
        return result;
    }

    private async UniTask<bool> OnAutoCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        int startI = i;
        for (int j = i; j < castables.Count; j++)
        {
            ICastable castable = castables[j];

            // Prefent first shot from a sequential norito from firing
            if (castable is INorito norito && 
                !norito.AutoCast &&             // Check if child is a sequential norito
                j != startI)                    // If it is, check if we're already processing it
                                                // (if j != startI, we just found the first child of the norito)
                return false;

            bool result = await castable.OnCastAsync(castData, parent, token);

            if (!result)
            {
                // A child either:  ran out of Ke or
                //                  We're processing a sequential norito so we should not increment
                //                  and we should terminate this thread to the root.
                return false;
            }
            IncrementI();
        }
        return true;
    }

    private async UniTask<bool> OnSequentialCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        ICastable castable = castables[i];
        bool result = await castable.OnCastAsync(castData, parent, token);
        if (!result &&                      // If we get the command to receive a break
            parent is INorito norito &&     // Check if the parent is a Norito
            !norito.CycleComplete)          // If it hasn't been completed yet (????), break
                                            // (honestly have no clue how this works lol trust the process i guess)
            return false;
        IncrementI();
        return CycleComplete;               // Returns true if we've gone through the entire norito
    }
}

[Serializable]
public class NoritoChargable : NoritoBase, ICastableChargable
{
    public async UniTask<bool> OnCastStart(CastingData castData, CancellationToken token)
    {
        foreach (ICastableChargable castable in castables)
        {
            if (!await castable.OnCastStart(castData, token))
                return false;
        }
        PreCast();
        return true;
    }

    public override async UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        foreach (ICastable castable in castables)
        {
           bool x = await castable.OnCastAsync(castData, this, token);
        }
        PostCast();
        return false;
    }

    public async UniTask<bool> OnCastEnd(CancellationToken token)
    {
        throw new NotImplementedException();
    }
}