using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface INoritoInventory
{
    public bool AddCastable(ICastable castable);
    public bool RemoveCastable(ICastable castable);
    public ICastable[] FindItems(ICastable castable);
    public ICastable[] FindItems(System.Func<ICastable, bool> filter);
    public ICastable[] GetAllItems();
    public void RemoveAllCastables();
}