using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public interface ICastable : ICastableData
{
    public bool OnCast(CastingData castData, MonoBehaviour mono);
}

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

public interface INorito
{
    public bool AutoCast { get; set; }
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

    public string Name => gokuName;

    public string Description => gokuDescription;

    public uint KeCost => keCost;

    public float InitialDelay => initialDelay + accumulatedInitialDelay;

    public float EndDelay => endDelay + accumulatedEndDelay;

    public float ActualEndDelay => endDelay + accumulatedEndDelay;

    public GameObject Model { get => prefab; set => prefab = value; }

    public bool OnCast(CastingData castData, MonoBehaviour mono)
    {
        Debug.Log("received");
        GameObject casted = UnityEngine.Object.Instantiate(prefab);
        casted.transform.position = castData.origin.position;
        RayMovement movement = casted.GetComponent<RayMovement>();
        castData.direction = castData.directionFunction(castData.origin);
        movement.velocity = castData.direction * castData.speed;
        movement.acceleration = castData.acceleration;
        movement.isMoving = true;

        IHitter hitter = casted.GetComponent<IHitter>();
        hitter.PreAttack(castData.direction, castData.ownerActor);
        return true;
    }

    public void OnOrder(IList orderedList, ICastableData parent, bool first = false, bool last = false)
    {
        if (first || last)
            SetDelays(parent, first);
        orderedList.Add(this);
    }

    private void SetDelays(ICastableData parent, bool first)
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
}

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

    public string Name => noritoName;
    public string Description => noritoDescription;
    public uint KeCost => totalKeCost;
    public float InitialDelay => initialDelay;
    public float EndDelay => endDelay;
    public float ActualEndDelay => endDelay + castables.Last().EndDelay;
    public GameObject Model { get => worldModelPrefab; set => worldModelPrefab = value; }
    public bool AutoCast { get => autoCast; set => autoCast = value; }

    public bool OnCast(CastingData castData, MonoBehaviour mono)
    {
        if (casting) return false;
        casting = true;

        if (autoCast)
        {
            OnAutoCast(castData, mono);
        }

        return true;
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

public interface ICastableDFS
{
    public void OnOrder(IList orderedList, ICastableData parent, bool first = false, bool last = false);
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

    public bool OnCast(CastingData castData, MonoBehaviour mono)
    {
        if (casting) return false;

        if (!ordered)
        {
            OrderChildren();
            ordered = true;
        }

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

    public void OnOrder(IList orderedList, ICastableData parent, bool first = false, bool last = false)
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

    private void SetDelays(ICastableData parent, bool first)
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