using UnityEngine;

public enum Hand
{
    Left = 0,
    Right = 1
}

public interface IHotbarDisplayable
{
    public string HotbarName { get; }
    public string HotbarDescription { get; }
    public GameObject WorldModel { get; }
}

public interface IInventoryController
{
    public IItemInventory Inventory { get; }
    public IHotbarDisplayable CurrentEquipped { get; }
    public bool AddItem(ItemBase itemBase);
    public bool AddItem(ItemBase itemBase, uint amount);
    public bool AddItem(ItemStack item);
    public bool RemoveItem(ItemBase itemBase);
    public bool RemoveItem(ItemBase itemBase, uint amount);
    public void TickInventory(Time deltaTime, IActor actor);
    public GameObject GetCurrentEquipped(Transform parent);
    public GameObject SetEquipped(int i, Transform parent);
    public GameObject GetNextEquipped(Transform parent);
    public GameObject GetPrevEquipped(Transform parent);
    public ItemStack[] FindItems(ItemBase itemBase);
    public ItemStack[] FindItems(ItemID id, ItemData data = null);
}
