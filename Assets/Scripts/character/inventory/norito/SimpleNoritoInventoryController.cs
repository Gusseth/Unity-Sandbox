using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

/*  List-based norito inventory system.
 *  Very simple.
 */
[Serializable]
public class SimpleNoritoInventoryController : MonoBehaviour, INoritoInventoryController
{
    [SerializeReference, SubclassSelector] List<ICastable> castables = new List<ICastable>();
    ICastable current;
    int i;
    INoritoInventory INoritoInventoryController.NoritoInventory => null;

    public GameObject GetCurrentEquipped(Transform parent)
    {
        return null;
    }

    public GameObject GetNextEquipped(Transform parent)
    {
        return SetEquipped((i + 1) % castables.Count, parent);
    }

    public GameObject GetPrevEquipped(Transform parent)
    {
        return SetEquipped((i - 1) % castables.Count, parent);
    }

    public GameObject SetEquipped(int i, Transform parent)
    {
        UpdateCurrent();
        if (i >= castables.Count || i < 0)
            return null;
        this.i = i;
        return GetCurrentEquipped(parent);
    }

    private void UpdateCurrent()
    {
        if (current != null)
            current.OnEquipOut();
    }

    bool INoritoInventoryController.OnCast(CastingData castData)
    {
        current.OnCastAsync(castData, null, this.GetCancellationTokenOnDestroy());
        return true;
    }

    void Awake()
    {
        current = castables[i];
        foreach (ICastable castable in castables)
            castable.CalculateKeCost();
    }
}
