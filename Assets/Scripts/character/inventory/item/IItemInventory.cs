using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface IItemInventory
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