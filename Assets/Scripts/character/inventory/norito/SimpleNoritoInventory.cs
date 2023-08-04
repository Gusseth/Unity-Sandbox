using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*  List-based norito inventory system.
 *  Very simple.
 */
[Serializable]
public class SimpleNoritoInventory : INoritoInventory
{
    [SerializeReference, SubclassSelector] List<ICastable> inventory = new List<ICastable>();

    public bool AddCastable(ICastable castable)
    {
        inventory.Add(castable);
        return true;
    }

    public ICastable[] FindItems(ICastable castable)
    {
        Func<ICastable, bool> filter = (c) =>
        {
            return c == castable;
        };
        return FindItems(filter);
    }

    public ICastable[] FindItems(Func<ICastable, bool> filter)
    {
        return inventory.Where(filter).ToArray();
    }

    public ICastable[] GetAllItems()
    {
        return inventory.ToArray();
    }

    public void RemoveAllCastables()
    {
        inventory.Clear();
    }

    public bool RemoveCastable(ICastable castable)
    {
        inventory.Remove(castable);
        return true;
    }
}
