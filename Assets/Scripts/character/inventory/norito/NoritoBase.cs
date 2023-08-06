using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public interface ICastableData
{
    public string Name { get; }
    public string Description { get; }
    public int KeCost { get; }
    public float InitialDelay { get; }
    public float EndDelay { get; }
    public float ActualEndDelay { get; }
    public bool Casting { get; }
    public GameObject Model { get; set; }
    public void CalculateKeCost();
}

public interface ICastable : ICastableData
{
    public Task<bool> OnCastAsync(CastingData castData, ICastable parent);
}

public interface INorito : ICastableData
{
    public bool AutoCast { get; set; }
    public bool CycleComplete { get; }
}

[Serializable]
public class Goku : ICastable, ICastableDFS
{
    public GameObject prefab;
    public string gokuName;
    public string gokuDescription;
    public int keCost = 0;
    public float initialDelay = 0;
    public float endDelay = 0;
    float accumulatedInitialDelay;
    float accumulatedEndDelay;
    bool casting;

    IMagicController prefabMagicController;

    public string Name => gokuName;
    public string Description => gokuDescription;
    public int KeCost => keCost;
    public float InitialDelay => initialDelay + accumulatedInitialDelay;
    public float EndDelay => endDelay + accumulatedEndDelay;
    public float ActualEndDelay => endDelay + accumulatedEndDelay;
    public GameObject Model { get => prefab; set => prefab = value; }
    public bool Casting => casting;


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

    public async Task<bool> OnCastAsync(CastingData castData, ICastable parent)
    {
        await TimeHelpers.WaitAsync(initialDelay);
        Debug.Log(Name);
        Initialize();
        if (prefabMagicController.CheckRequirements(castData))
        {
            GameObject casted = UnityEngine.Object.Instantiate(prefab);
            IMagicController magicController = casted.GetComponent<IMagicController>();
            magicController.Init();
            if (!magicController.OnCast(castData))
            {
                return false;
            }
            else
            {
                await TimeHelpers.WaitAsync(endDelay);
                return true;
            }
        }
        return false;
    }

    public void PostCast(ICastable parent = null)
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
public class Norito : ICastable, INorito
{
    public static readonly float hardcodedDelay = 0.15f;

    [SerializeReference, SubclassSelector] public List<ICastable> castables = new List<ICastable>();
    public GameObject worldModelPrefab;
    public int totalKeCost = 0;
    public float initialDelay = 0;
    public float endDelay = 0;
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
    public float InitialDelay => initialDelay;
    public float EndDelay => endDelay;
    public float ActualEndDelay => endDelay + castables.Last().EndDelay;
    public GameObject Model { get => worldModelPrefab; set => worldModelPrefab = value; }
    public bool Casting => casting;
    public bool AutoCast { get => autoCast; set => autoCast = value; }
    public bool CycleComplete => cycleComplete();
    public bool OnLastCastable => false;


    private bool cycleComplete()
    {
        return i == 0 && finishedAtLeastOnce;
    }

    public async Task<bool> OnCastAsync(CastingData castData, ICastable parent)
    {
        if (casting) return false;

        if (!keAlreadyCalculated)
            CalculateKeCost();

        PreCast();
        if (i == 0)
        {
            // Start of the norito
            await TimeHelpers.WaitAsync(initialDelay);
        }

        bool result;

        if (autoCast)
            result = await OnAutoCastAsync(castData, this);
        else
            result = await OnSequentialCastAsync(castData, this);

        if (finishedAtLeastOnce)
        {
            // End of the norito
            await TimeHelpers.WaitAsync(endDelay);
            finishedAtLeastOnce = false;
        }

        PostCast();
        return result;
    }

    private async Task<bool> OnAutoCastAsync(CastingData castData, ICastable parent)
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

            bool result = await castable.OnCastAsync(castData, parent);

            if (!result)
            {
                // A child either:  ran out of 'Ke' or
                //                  We're processing a sequential norito so we should not increment
                //                  and we should terminate this thread to the root.
                return false;
            }
            IncrementI();
        }
        return true;
    }

    private async Task<bool> OnSequentialCastAsync(CastingData castData, ICastable parent)
    {
        ICastable castable = castables[i];
        bool result = await castable.OnCastAsync(castData, parent);
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