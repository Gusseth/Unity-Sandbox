using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class TreeInventoryEntry : ItemStack
{
    public ICollection<TreeInventoryEntry> variants;
}

[Serializable]
public class TreeInventory : IInventory
{
    [SerializeField] IDictionary<ItemID, TreeInventoryEntry> inventory;

    public bool AddItem(ItemBase item, uint amount)
    {
        TreeInventoryEntry entry = inventory[item.Id];
        if (entry != null)
        {
            entry.amountTotal += amount;
            if (entry.variants != null)
            {
                AddVariant(entry, item, amount);
            }
            else
            {
                entry.variants = new List<TreeInventoryEntry>();
                //entry.variants.
            }
        }
        else
        {
            inventory.Add(item.Id, new TreeInventoryEntry());
        }
        return true;
    }

    public bool AddItem(ItemStack entry)
    {
        throw new NotImplementedException();
    }

    public ItemStack[] FindItems(ItemBase item)
    {
        throw new System.NotImplementedException();
    }

    public ItemStack[] FindItems(ItemID id, ItemData data = null)
    {
        TreeInventoryEntry entry = inventory[id];
        if (entry == null)
            return Array.Empty<ItemStack>();

        throw new System.NotImplementedException();
    }

    public ItemStack[] FindItems(Func<ItemStack, bool> filter)
    {
        throw new NotImplementedException();
    }

    public ItemStack[] GetAllItems()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveAllItems(bool exceptImportant = true)
    {
        throw new NotImplementedException();
    }

    public bool RemoveItem(ItemBase item, uint amount)
    {
        throw new System.NotImplementedException();
    }


    private void AddVariant(TreeInventoryEntry rootEntry, ItemBase item, uint amount)
    {
        ICollection<TreeInventoryEntry> variants = rootEntry.variants;
        foreach (TreeInventoryEntry entry in variants)
        {
            if (entry.itemBase.Data == item.Data)
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
