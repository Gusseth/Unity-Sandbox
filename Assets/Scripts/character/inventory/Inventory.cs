using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleInventory : IInventory
{
    [SerializeField] ICollection<InventoryEntry> inventory;

    public SimpleInventory() { 
        inventory = new List<InventoryEntry>();
    }

    public bool AddItem(Item item, uint amount)
    {
        foreach (InventoryEntry entry in inventory)
        {
            if (item == entry.item)
            {
                entry.amountTotal += amount;
                return true;
            }
        }

        // No hits, must make a new entry
        inventory.Add(new InventoryEntry
        {
            item = item,
            amountTotal = 1,
            entryName = item.Name,
            entryDescription = item.Description
        });

        return true;
    }

    public InventoryEntry[] FindItemsWithAttributes(ItemData data)
    {
        List<InventoryEntry> items = new List<InventoryEntry>();

        foreach (InventoryEntry entry in inventory)
        {
            if (entry.item.Data == data)
            {
                items.Add(entry);
            }
        }

        return items.ToArray();
    }

    public InventoryEntry[] FindItemsWithID(ItemID id)
    {
        List<InventoryEntry> items = new List<InventoryEntry>();

        foreach (InventoryEntry entry in inventory)
        {
            if (entry.item.Id == id)
            {
                items.Add(entry);
            }
        }

        return items.ToArray();
    }

    public InventoryEntry[] GetAllItems()
    {
        return inventory.ToArray();
    }

    public void RemoveAllItems(bool exceptImportant = true)
    {
        if (exceptImportant)
        {
            foreach (InventoryEntry entry in inventory)
            {
                if (!entry.item.Data.important)
                {
                    inventory.Remove(entry);
                }
            }
        }
        else
        {
            inventory.Clear();
        }
    }

    public bool RemoveItem(Item item, uint amount)
    {
        foreach (InventoryEntry entry in inventory)
        {
            if (entry.item == item)
            {
                // You're removing too much!
                if (entry.amountTotal < amount)
                    return false;

                entry.amountTotal -= amount;
                if (entry.amountTotal <= 0)
                {
                    inventory.Remove(entry);
                }
            }
            return true;
        }
        return false;
    }
}

[Serializable]
class TreedInventoryEntry : InventoryEntry
{
    public ICollection<TreedInventoryEntry> variants;
}

public class TreedInventory : IInventory
{
    [SerializeField] IDictionary<ItemID, TreedInventoryEntry> inventory;

    public bool AddItem(Item item, uint amount)
    {
        TreedInventoryEntry entry = inventory[item.Id];
        if (entry != null)
        {
            entry.amountTotal += amount;
            if (entry.variants != null)
            {
                AddVariant(entry, item, amount);
            }
            else
            {
                entry.variants = new List<TreedInventoryEntry>();
                //entry.variants.
            }
        }
        else
        {
            inventory.Add(item.Id, new TreedInventoryEntry());
        }
        return true;
    }

    public InventoryEntry[] FindItemsWithAttributes(ItemData data)
    {
        throw new System.NotImplementedException();
    }

    public InventoryEntry[] FindItemsWithID(ItemID id)
    {
        TreedInventoryEntry entry = inventory[id];
        if (entry == null)
            return Array.Empty<InventoryEntry>();

        throw new System.NotImplementedException();
    }

    public InventoryEntry[] GetAllItems()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveAllItems(bool exceptImportant = true)
    {
        throw new NotImplementedException();
    }

    public bool RemoveItem(Item item, uint amount)
    {
        throw new System.NotImplementedException();
    }


    private void AddVariant(TreedInventoryEntry rootEntry, Item item, uint amount)
    {
        ICollection<TreedInventoryEntry> variants = rootEntry.variants;
        foreach (TreedInventoryEntry entry in variants)
        {
            if (entry.item.Data == item.Data)
            {
                entry.amountTotal += amount;

                if (entry.variants != null)
                {
                    AddVariant(entry, item, amount);
                }
                return;
            }
        }
    }
}