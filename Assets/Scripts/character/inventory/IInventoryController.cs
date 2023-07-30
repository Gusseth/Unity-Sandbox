using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum Hand
{
    Left = 0,
    Right = 1
}

public interface IInventoryController
{
    public IInventory Inventory { get; }
    public bool AddItem(ItemBase itemBase);
    public bool AddItem(ItemBase itemBase, uint amount);
    public bool AddItem(ItemStack item);
    public bool RemoveItem(ItemBase itemBase);
    public bool RemoveItem(ItemBase itemBase, uint amount);
    public void TickInventory(Time deltaTime, IActor actor);
    public GameObject GetEquipped(Hand hand, Transform parent);
    public GameObject GetNextEquipped(Hand hand, Transform parent);
    public GameObject GetPrevEquipped(Hand hand, Transform parent);
    public ItemStack[] FindItems(ItemBase itemBase);
    public ItemStack[] FindItems(ItemID id, ItemData data = null);
}

public interface IInventory
{
    public bool AddItem(ItemBase itemBase, uint amount);
    public bool AddItem(ItemStack item);
    public bool RemoveItem(ItemBase item, uint amount);
    public ItemStack[] FindItems(ItemBase itemBase);
    public ItemStack[] FindItems(ItemID id, ItemData data = null);
    public ItemStack[] FindItems(System.Func<ItemStack, bool> filter);
    public ItemStack[] GetAllItems();
    public void RemoveAllItems(bool exceptImportant = true);
}
