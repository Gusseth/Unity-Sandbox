﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum Hand
{
    Left = 0,
    Right = 1
}

[Serializable]
public class InventoryEntry
{
    public Item item;
    public GameObject model;
    public uint amountTotal;
    public string entryName;
    public string entryDescription;
}

public interface IInventoryController
{
    public IInventory Inventory { get; }
    public bool AddItem(Item item);
    public bool AddItem(Item item, uint amount);
    public bool RemoveItem(Item item);
    public bool RemoveItem(Item item, uint amount);
    public void TickInventory(Time deltaTime, IActor actor);
    public GameObject GetEquipped(Hand hand, Transform parent);
    public GameObject GetNextEquipped(Hand hand, Transform parent);
    public GameObject GetPrevEquipped(Hand hand, Transform parent);
    public Item[] FindItemsWithID(ItemID id);
    public Item[] FindItemsWithAttributes(ItemData data);
}

public interface IInventory
{
    public bool AddItem(Item item, uint amount);
    public bool RemoveItem(Item item, uint amount);
    public InventoryEntry[] FindItemsWithID(ItemID id);
    public InventoryEntry[] FindItemsWithAttributes(ItemData data);
    public InventoryEntry[] GetAllItems();
    public void RemoveAllItems(bool exceptImportant = true);
}
