using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface IInventoryController
{
    public IInventory Inventory { get; }
    public bool AddItem(Item item);
    public bool AddItem(Item item, uint amount);
    public bool RemoveItem(Item item);
    public bool RemoveItem(Item item, uint amount);
    public void TickInventory(Time deltaTime, IHaveInventory actor);
    public Item[] FindItemsWithID(ItemID id);
    public Item[] FindItemsWithAttributes(ItemData data);
}

public interface IInventory
{
    public bool AddItem(Item item, uint amount);
    public bool RemoveItem(Item item, uint amount);
    public Item[] FindItemsWithID(ItemID id);
    public Item[] FindItemsWithAttributes(ItemData data);
    public Item[] GetAllItems();
}
