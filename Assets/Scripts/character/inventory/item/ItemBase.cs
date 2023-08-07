using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class ItemBase
{
    public ItemID Id;
    public ItemData Data;
    public bool Stackable { get; set; }
    public string BaseName { get; set; }
    public string BaseDescription { get; set; }
}

[Serializable]
public class ItemData
{
    public bool important;
}

[Serializable]
public class ItemID
{
    public string value;
}

[Serializable]
public class ItemStack : IInteractableData, IHotbarDisplayable
{
    public ItemBase itemBase;
    public GameObject worldModelPrefab;
    public uint amountTotal;
    public string stackName;
    public string stackDescription;

    public string Name => stackName;
    public string Description => stackDescription;
    public uint Amount => amountTotal;
    public string HotbarName => Name;
    public string HotbarDescription => Description;
    public GameObject WorldModel => worldModelPrefab;
}