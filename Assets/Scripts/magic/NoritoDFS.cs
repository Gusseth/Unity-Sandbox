using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public interface ICastableDFS
{
    public void OnOrder(IList orderedList, INorito parent, bool first = false, bool last = false);
}

/*
 * TODO: Match the DFS-based solution's features with the other tree-based solution
 *       It should improve performance over the current implementation, 
 *       but my brain is too small to think of an algorithm.
 *       
 *       FEATURES:
 *          Dynamically switch between autoCast and sequential modes depending on the Norito's autoCast
 *          ie.:    AutoCast-ing Norito should automatically cast its children while
 *                  Sequential Norito must call OnCast() again to move to the next spell
 */

[Serializable]
public class NoritoDFS : ICastable, INorito
{
    [SerializeReference, SubclassSelector] public List<ICastableDFS> children = new List<ICastableDFS>();
    [SerializeReference, SubclassSelector] public List<ICastable> orderedCastables = new List<ICastable>();
    public GameObject worldModelPrefab;
    public int totalKeCost = 0;
    public int initialDelay = 0;
    public int endDelay = 0;
    public string noritoName;
    public string noritoDescription;
    public bool autoCast = true;


    [SerializeField] bool casting = false;
    [SerializeField] bool ordered = false;
    [SerializeField] int i;

    public string Name => noritoName;
    public string Description => noritoDescription;
    public int KeCost => totalKeCost;
    public int InitialDelay => initialDelay;
    public int EndDelay => endDelay;
    public int ActualEndDelay => endDelay + orderedCastables.Last().EndDelay;
    public GameObject Model { get => worldModelPrefab; set => worldModelPrefab = value; }
    public bool Casting => casting;
    public bool AutoCast { get => autoCast; set => autoCast = value; }
    public bool CycleComplete => autoCast ? casting : i == 0;
    public bool OnLastCastable => i == children.Count - 1;

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
            //castable.OnCast(castData, mono);
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

    public UniTask<bool> OnCastAsync(CastingData castData, ICastable parent, CancellationToken token)
    {
        // TODO: Feature match with the tree implementation
        return UniTask.FromResult(true);
    }

    private void OnSequentialCast(CastingData castData, MonoBehaviour mono)
    {
        ICastable castable = orderedCastables[i];

        void cast()
        {
            //castable.OnCast(castData, mono);
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

    public void PostCast()
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

    public void CalculateKeCost()
    {
        totalKeCost = 0;
        foreach (ICastableData node in orderedCastables)
        {
            totalKeCost += node.KeCost;
        }
    }

    public UniTask<bool> OnCastStart(CastingData castData, ICastable parent, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public UniTask<bool> OnCastEnd(CastingData castData, ICastable parent, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public void OnEquipOut()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class NoritoDFSNode : ICastableDFS, ICastableData, INorito
{
    [SerializeReference, SubclassSelector] public List<ICastableDFS> children = new List<ICastableDFS>();
    public GameObject worldModelPrefab;
    public int totalKeCost = 0;
    public int initialDelay = 0;
    public int endDelay = 0;
    public string noritoName;
    public string noritoDescription;
    public bool autoCast = true;
    [SerializeField] bool cycleComplete = true;

    int accumulatedEndDelay = 0;
    int accumulatedInitialDelay = 0;

    public string Name => noritoName;
    public string Description => noritoDescription;
    public int KeCost => totalKeCost;
    public int InitialDelay => initialDelay + accumulatedInitialDelay;
    public int EndDelay => endDelay;
    public int ActualEndDelay => endDelay + accumulatedEndDelay;
    public GameObject Model { get => worldModelPrefab; set => worldModelPrefab = value; }
    public bool Casting => false;
    public bool AutoCast { get => autoCast; set => autoCast = value; }
    public bool CycleComplete => cycleComplete;
    public bool OnLastCastable => cycleComplete;

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

    public void CalculateKeCost()
    {
        totalKeCost = 0;
        foreach (ICastableData node in children)
        {
            totalKeCost += node.KeCost;
        }
    }
}