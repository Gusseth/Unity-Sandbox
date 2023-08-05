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
    public uint KeCost { get; }
    public float InitialDelay { get; }
    public float EndDelay { get; }
    public float ActualEndDelay { get; }
    public GameObject Model { get; set; }
}

public interface ICastable : ICastableData
{
    public bool OnCast(CastingData castData, MonoBehaviour mono);
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
    public uint keCost = 0;
    public float initialDelay = 0;
    public float endDelay = 0;
    float accumulatedInitialDelay;
    float accumulatedEndDelay;

    IMagicController prefabMagicController;

    public string Name => gokuName;

    public string Description => gokuDescription;

    public uint KeCost => keCost;

    public float InitialDelay => initialDelay + accumulatedInitialDelay;

    public float EndDelay => endDelay + accumulatedEndDelay;

    public float ActualEndDelay => endDelay + accumulatedEndDelay;

    public GameObject Model { get => prefab; set => prefab = value; }

    public bool OnCast(CastingData castData, MonoBehaviour mono)
    {
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
    public uint totalKeCost = 0;
    public float initialDelay = 0;
    public float endDelay = 0;
    public string noritoName;
    public string noritoDescription;
    public bool autoCast = true;

    [SerializeField] bool casting = false;
    [SerializeField] int i;

    public string Name => noritoName;
    public string Description => noritoDescription;
    public uint KeCost => totalKeCost;
    public float InitialDelay => initialDelay;
    public float EndDelay => endDelay;
    public float ActualEndDelay => endDelay + castables.Last().EndDelay;
    public GameObject Model { get => worldModelPrefab; set => worldModelPrefab = value; }
    public bool AutoCast { get => autoCast; set => autoCast = value; }
    public bool CycleComplete => autoCast ? casting : i == 0;

    public bool OnCast(CastingData castData, MonoBehaviour mono)
    {
        if (casting) return false;
        casting = true;

        if (autoCast)
        {
            OnAutoCast(castData, mono);
        }
        else
        {
            OnSequentialCast(castData, mono);
        }

        return true;
    }

    private void OnSequentialCast(CastingData castData, MonoBehaviour mono)
    {
        ICastable castable = castables[i];

        void cast()
        {
            castable.OnCast(castData, mono);
        }

        TimeHelpers.StaggeredCoroutine(mono, cast, castable.InitialDelay, castable.ActualEndDelay, PostCast);

        INorito child = castable as INorito;
        if (child != null)
        {
            if (!child.CycleComplete) return;
        }

        if (i < castables.Count - 1)
            i++;
        else
            i = 0;
    }

    private void OnAutoCast(CastingData castData, MonoBehaviour mono)
    {
        void BodyFunction(ICastable castable)
        {
            castable.OnCast(castData, mono);
        };

        TimeHelpers.StaggeredEnumerationCoroutine(mono, DelayFunction, BodyFunction, castables, PostCast);
    }

    private (float, float) DelayFunction(ICastable castable)
    {
        return (castable.InitialDelay, castable.ActualEndDelay);
    }

    private void PostCast()
    {
        casting = false;
    }

    private void NextCastable(AbstractActorBase actor, ICastable castable)
    {
    }

    private void CalculateKeCost(ICollection<ICastable> castables)
    {
        totalKeCost = 0;
        foreach (ICastable castable in castables)
        {
            totalKeCost += castable.KeCost;
        }
    }
}