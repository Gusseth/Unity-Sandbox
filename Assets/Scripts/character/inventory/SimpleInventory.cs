using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*  List-based inventory system.
 *  Very simple.
 */
[Serializable]
public class SimpleInventory : IInventory
{
    [SerializeField] List<ItemStack> inventory;

    public SimpleInventory() { 
        inventory = new List<ItemStack>();
    }

    public bool AddItem(ItemBase itemBase, uint amount)
    {
        return AddItem(new ItemStack
        {
            itemBase = itemBase,
            amountTotal = amount,
            stackName = itemBase.BaseName,
            stackDescription = itemBase.BaseDescription
        });
    }

    public bool AddItem(ItemStack item)
    {
        foreach (ItemStack i in inventory)
        {
            if (item.itemBase == i.itemBase)
            {
                i.amountTotal += item.amountTotal;
                return true;
            }
        }

        // No hits, must make a new entry
        inventory.Add(item);

        return true;
    }

    public ItemStack[] FindItems(ItemBase itemBase)
    {
        return FindItems(itemBase.Id, itemBase.Data);
    }

    public ItemStack[] FindItems(ItemID id, ItemData data = null)
    {
        // Make a filter where the ids must match AND the item data must match (if defined)
        Func<ItemStack, bool> filter = (item) =>
            {
                return  item.itemBase.Id.value == id.value &&
                        (data == null || item.itemBase.Data == data);
            };

        return FindItems(filter);
    }

    public ItemStack[] FindItems(Func<ItemStack, bool> filter)
    {
        List<ItemStack> items = new List<ItemStack>();

        foreach (ItemStack item in inventory)
        {
            if (filter(item))
            {
                items.Add(item);
            }
        }

        return items.ToArray();
    }

    public ItemStack[] GetAllItems()
    {
        return inventory.ToArray();
    }

    public void RemoveAllItems(bool exceptImportant = true)
    {
        if (exceptImportant)
        {
            foreach (ItemStack item in inventory)
            {
                if (!item.itemBase.Data.important)
                {
                    inventory.Remove(item);
                }
            }
        }
        else
        {
            inventory.Clear();
        }
    }

    public bool RemoveItem(ItemBase itemBase, uint amount)
    {
        foreach (ItemStack item in inventory)
        {
            if (item.itemBase == itemBase)
            {
                // You're removing too much!
                if (item.amountTotal < amount)
                    return false;

                item.amountTotal -= amount;
                if (item.amountTotal <= 0)
                {
                    inventory.Remove(item);
                }
            }
            return true;
        }
        return false;
    }
}
