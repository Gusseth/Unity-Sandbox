using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryController : MonoBehaviour, IInventoryController, INoritoInventoryController
{
    [SerializeReference, SubclassSelector] List<IHotbarDisplayable> equippedHotbar = new List<IHotbarDisplayable>();
    [SerializeField] int i = 0;
    [SerializeField] SimpleItemInventory inventory;
    [SerializeField] SimpleNoritoInventory noritoInventory;
    [SerializeField] HotbarUI hotbarUI;
    [SerializeField] TextMeshProUGUI hotbarNameIndicator;

    public IItemInventory Inventory => inventory;

    public INoritoInventory NoritoInventory => noritoInventory;

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

    public GameObject GetCurrentEquipped(Transform parent)
    {
        return GetEquippedModel(parent);
    }

    private GameObject GetEquippedModel(Transform parent)
    {
        if (hotbarUI.Slots.Count == 0)
        {
            hotbarUI.SetHotbar(equippedHotbar);
        }
        hotbarUI.SelectSlot(i);
        return Instantiate(equippedHotbar[i].WorldModel, parent);
    }

    public GameObject SetEquipped(int i, Transform parent)
    {
        if (i >= equippedHotbar.Count || i < 0)
            return null;
        this.i = i;
        return GetEquippedModel(parent);
    }

    public GameObject GetNextEquipped(Transform parent)
    {
        if (i == equippedHotbar.Count - 1)
            i = 0;
        else
            i++;
        return GetEquippedModel(parent);
    }

    public GameObject GetPrevEquipped(Transform parent)
    {
        if (i == 0)
            i = equippedHotbar.Count - 1;
        else
            i--;
        return GetEquippedModel(parent);
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
    void Awake()
    {
        hotbarUI.SetHotbar(equippedHotbar);
        foreach(IHotbarDisplayable displayable in equippedHotbar)
        {
            if (displayable is ICastable castable)
                castable.CalculateKeCost();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool OnCast(CastingData castData)
    {
        if (equippedHotbar[i] is ICastable castable)
        {
            castable.OnCastAsync(castData, null, this.GetCancellationTokenOnDestroy());
            return true;
        }
        return false;
    }
}
