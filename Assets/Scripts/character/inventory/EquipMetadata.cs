﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquippableType
{
    weaponMelee,
    weaponMagic
}

public interface IEquipMetadata
{
    public EquippableType EquippableType { get; }
    public GameObject gameObject { get; }
    public void OnEquip(IInventoryController inventoryController, INoritoInventoryController noritoController, IActor actor);
    public void OnUnequip(IInventoryController inventoryController, INoritoInventoryController noritoController, IActor actor);
}

public class EquipMetadata : MonoBehaviour, IEquipMetadata
{
    [SerializeField] EquippableType equippableType;

    public EquippableType EquippableType => equippableType;

    public void OnEquip(IInventoryController inventoryController, INoritoInventoryController noritoController, IActor actor)
    {
        
    }

    public void OnUnequip(IInventoryController inventoryController, INoritoInventoryController noritoController, IActor actor)
    {
        Destroy(gameObject);
    }
}