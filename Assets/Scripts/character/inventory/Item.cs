using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public abstract class Item
{
    public ItemID Id;
    public ItemData Data;
    public bool Stackable { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

}

[Serializable]
public class ItemData
{
    public bool important;
}

[Serializable]
public class ItemID
{
    
}