using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface INoritoInventoryController
{
    public INoritoInventory NoritoInventory { get; }
    public IHotbarDisplayable CurrentEquipped { get; }
    public bool OnCast(CastingData castData);
    public GameObject GetCurrentEquipped(Transform parent);
    public GameObject SetEquipped(int i, Transform parent);
    public GameObject GetNextEquipped(Transform parent);
    public GameObject GetPrevEquipped(Transform parent);
}