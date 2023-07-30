using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour, IInventoryController
{
    [SerializeField] GameObject[] quickEquip;
    [SerializeField] int i = 0;
    [SerializeField] SimpleInventory inventory;

    public IInventory Inventory => inventory;

    public bool AddItem(ItemBase itemBase)
    {
        return inventory.AddItem(itemBase, 1);
    }

    public bool AddItem(ItemBase itemBase, uint amount)
    {
        return inventory.AddItem(itemBase, amount);
    }
    public bool AddItem(ItemStack item)
    {
        return inventory.AddItem(item);
    }

    public ItemStack[] FindItems(ItemBase itemBase)
    {
        return inventory.FindItems(itemBase);
    }

    public ItemStack[] FindItems(ItemID id, ItemData data = null)
    {
        return inventory.FindItems(id, data);
    }

    public GameObject GetEquipped(Hand hand, Transform parent)
    {
        return Instantiate(quickEquip[i], parent);
    }

    public GameObject GetNextEquipped(Hand hand, Transform parent)
    {
        if (i == quickEquip.Length - 1)
            i = 0;
        else
            i++;
        return Instantiate(quickEquip[i], parent);
    }

    public GameObject GetPrevEquipped(Hand hand, Transform parent)
    {
        if (i == 0)
            i = quickEquip.Length - 1;
        else
            i--;
        return Instantiate(quickEquip[i], parent);
    }

    public bool RemoveItem(ItemBase itemBase)
    {
        return inventory.RemoveItem(itemBase, 1);
    }

    public bool RemoveItem(ItemBase itemBase, uint amount)
    {
        return inventory.RemoveItem(itemBase, amount);
    }

    public void TickInventory(Time deltaTime, IActor actor)
    {
        throw new System.NotImplementedException();
    }

    private ItemBase[] ExtractEntries(ItemStack[] items)
    {
        ItemBase[] itemBases = new ItemBase[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            itemBases[i] = items[i].itemBase;
        }
        return itemBases;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
