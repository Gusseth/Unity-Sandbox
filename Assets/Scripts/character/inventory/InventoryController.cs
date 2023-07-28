using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour, IInventoryController
{
    [SerializeField] GameObject[] quickEquip;
    [SerializeField] int i = 0;
    IInventory inventory;

    public IInventory Inventory => inventory;

    public bool AddItem(Item item)
    {
        return inventory.AddItem(item, 1);
    }

    public bool AddItem(Item item, uint amount)
    {
        return inventory.AddItem(item, amount);
    }

    public Item[] FindItemsWithAttributes(ItemData data)
    {
        return inventory.FindItemsWithAttributes(data);
    }

    public Item[] FindItemsWithID(ItemID id)
    {
        return inventory.FindItemsWithID(id);
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

    public bool RemoveItem(Item item)
    {
        return inventory.RemoveItem(item, 1);
    }

    public bool RemoveItem(Item item, uint amount)
    {
        return inventory.RemoveItem(item, amount);
    }

    public void TickInventory(Time deltaTime, IActor actor)
    {
        throw new System.NotImplementedException();
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
