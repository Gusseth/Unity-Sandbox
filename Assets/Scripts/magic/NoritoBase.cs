using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

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
public class Goku : ICastable, ICastableDFS, IHotbarDisplayable
{
    public GameObject prefab;
    public GameObject worldModel;
    public string gokuName;
    public string gokuDescription;
    public int keCost = 0;
    public int initialDelay = 0;
    public int endDelay = 0;
    int accumulatedInitialDelay;
    int accumulatedEndDelay;
    bool casting;

    IMagicController prefabMagicController;

    public string Name => gokuName;
    public string Description => gokuDescription;
    public int KeCost => keCost;
    public int InitialDelay => initialDelay + accumulatedInitialDelay;
    public int EndDelay => endDelay + accumulatedEndDelay;
    public int ActualEndDelay => endDelay + accumulatedEndDelay;  // deprecated
    public GameObject Model { get => prefab; set => prefab = value; }
    public bool Casting => casting;
    public string HotbarName => Name;
    public string HotbarDescription => Description;
    public GameObject WorldModel => worldModel;

    public bool OnCast(CastingData castData, MonoBehaviour mono)
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

    public async UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        await UniTask.Delay(initialDelay, cancellationToken: token);
        Debug.Log(Name);
        Initialize();
        if (prefabMagicController.CheckRequirements(castData) && !token.IsCancellationRequested)
        {
            casting = true;
            GameObject casted = UnityEngine.Object.Instantiate(prefab);
            IMagicController magicController = casted.GetComponent<IMagicController>();
            magicController.Init();
            if (magicController.OnCast(castData))
            {
                await UniTask.Delay(endDelay, cancellationToken: token);
                casting = false;
                return true;
            }
        }
        return false;
    }

    public void PostCast()
    {

    }

    public void OnOrder(IList orderedList, INorito parent, bool first = false, bool last = false)
    {
        if (first || last)
            SetDelays(parent, first);
        orderedList.Add(this);
        Initialize();
    }

    private void SetDelays(INorito parent, bool first)
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

    private void Initialize()
    {
        prefabMagicController ??= prefab.GetComponent<IMagicController>();
    }

    public void CalculateKeCost()
    {
        Initialize();
        keCost = prefabMagicController.KeCost;
    }
}

/*
 * Issues with SubclassSelector? Download the package here:
 * https://github.com/mackysoft/Unity-SerializeReferenceExtensions
 */

[Serializable]
public class Norito : ICastable, INorito, IHotbarDisplayable
{
    [SerializeReference, SubclassSelector] public List<ICastable> castables = new List<ICastable>();
    public GameObject worldModelPrefab;
    public int totalKeCost = 0;
    public int initialDelay = 0;
    public int endDelay = 0;
    public string noritoName;
    public string noritoDescription;
    public bool autoCast = true;

    [SerializeField] bool casting = false;
    [SerializeField] bool finishedAtLeastOnce = false;
    [SerializeField] int i;

    bool keAlreadyCalculated = false;

    public string Name => noritoName;
    public string Description => noritoDescription;
    public int KeCost => totalKeCost;
    public int InitialDelay => initialDelay;
    public int EndDelay => endDelay;
    public int ActualEndDelay => endDelay + castables.Last().EndDelay;
    public GameObject Model { get => worldModelPrefab; set => worldModelPrefab = value; }
    public bool Casting => casting;
    public bool AutoCast { get => autoCast; set => autoCast = value; }
    public bool CycleComplete => cycleComplete();
    public bool OnLastCastable => false;
    public string HotbarName => Name;
    public string HotbarDescription => Description;
    public GameObject WorldModel => worldModelPrefab;

    private bool cycleComplete()
    {
        return i == 0 && finishedAtLeastOnce;
    }

    public async UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        if (casting) return false;

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

    private void IncrementI()
    {
        if (i < castables.Count - 1)
            i++;
        else
        {
            i = 0;
            finishedAtLeastOnce = true;
        }
    }

    private void PreCast()
    {
        casting = true;
    }

    public void PostCast()
    {
        casting = false;
    }

    public void CalculateKeCost()
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