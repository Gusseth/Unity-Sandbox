using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public interface ICastableDFS
{
    public void OnOrder(IList orderedList, INorito parent, bool first = false, bool last = false);
}

[Serializable]
public class NoritoDFS : ICastable, INorito
{
    [SerializeReference, SubclassSelector] public List<ICastableDFS> children = new List<ICastableDFS>();
    [SerializeReference, SubclassSelector] public List<ICastable> orderedCastables = new List<ICastable>();
    public GameObject worldModelPrefab;
    public uint totalKeCost = 0;
    public float initialDelay = 0;
    public float endDelay = 0;
    public string noritoName;
    public string noritoDescription;
    public bool autoCast = true;
    [SerializeField] bool cycleComplete = true;


    [SerializeField] bool casting = false;
    [SerializeField] bool ordered = false;
    [SerializeField] int i;

    public string Name => noritoName;
    public string Description => noritoDescription;
    public uint KeCost => totalKeCost;
    public float InitialDelay => initialDelay;
    public float EndDelay => endDelay;
    public float ActualEndDelay => endDelay + orderedCastables.Last().EndDelay;
    public GameObject Model { get => worldModelPrefab; set => worldModelPrefab = value; }
    public bool AutoCast { get => autoCast; set => autoCast = value; }
    public bool CycleComplete => cycleComplete;

    public bool OnCast(CastingData castData, MonoBehaviour mono)
    {
        if (casting) return false;

        if (!ordered)
        {
            OrderChildren();
            ordered = true;
        }

        if (orderedCastables.Count == 0)
            return false;

        casting = true;
        void BodyFunction(ICastable castable)
        {
            castable.OnCast(castData, mono);
        };

        if (autoCast)
        {
            TimeHelpers.StaggeredEnumerationCoroutine(mono, DelayFunction, BodyFunction, orderedCastables, PostCast);
        }
        else
        {
            OnSequentialCast(castData, mono);
        }

        return true;
    }

    private void OnSequentialCast(CastingData castData, MonoBehaviour mono)
    {
        ICastable castable = orderedCastables[i];

        void cast()
        {
            castable.OnCast(castData, mono);
        }

        TimeHelpers.StaggeredCoroutine(mono, cast, castable.InitialDelay, castable.ActualEndDelay, PostCast);

        if (i < orderedCastables.Count - 1)
            i++;
        else
            i = 0;
    }

    private (float, float) DelayFunction(ICastable castable)
    {
        return (castable.InitialDelay, castable.ActualEndDelay);
    }

    private void PostCast()
    {
        casting = false;
    }

    public void OrderChildren()
    {
        orderedCastables.Clear();
        for (int i = 0; i < children.Count; i++)
        {
            ICastableDFS node = children[i];
            node.OnOrder(orderedCastables, this, i == 0, i == children.Count - 1);
        }
        CalculateKeCost();
    }

    private void CalculateKeCost()
    {
        totalKeCost = 0;
        foreach (ICastableData node in orderedCastables)
        {
            totalKeCost += node.KeCost;
        }
    }
}

[Serializable]
public class NoritoDFSNode : ICastableDFS, ICastableData, INorito
{
    [SerializeReference, SubclassSelector] public List<ICastableDFS> children = new List<ICastableDFS>();
    public GameObject worldModelPrefab;
    public uint totalKeCost = 0;
    public float initialDelay = 0;
    public float endDelay = 0;
    public string noritoName;
    public string noritoDescription;
    public bool autoCast = true;
    [SerializeField] bool cycleComplete = true;

    float accumulatedEndDelay = 0;
    float accumulatedInitialDelay = 0;

    public string Name => noritoName;
    public string Description => noritoDescription;
    public uint KeCost => totalKeCost;
    public float InitialDelay => initialDelay + accumulatedInitialDelay;
    public float EndDelay => endDelay;
    public float ActualEndDelay => endDelay + accumulatedEndDelay;
    public GameObject Model { get => worldModelPrefab; set => worldModelPrefab = value; }
    public bool AutoCast { get => autoCast; set => autoCast = value; }

    public bool CycleComplete => cycleComplete;

    public void OnOrder(IList orderedList, INorito parent, bool first = false, bool last = false)
    {
        if (first | last)
        {
            SetDelays(parent, first);
        }
        
        
        CalculateKeCost();

        for (int i = 0; i < children.Count; i++)
        {
            ICastableDFS node = children[i];
            node.OnOrder(orderedList, this, i == 0, i == children.Count - 1);
        }
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

    private void CalculateKeCost()
    {
        totalKeCost = 0;
        foreach (ICastableData node in children)
        {
            totalKeCost += node.KeCost;
        }
    }
}