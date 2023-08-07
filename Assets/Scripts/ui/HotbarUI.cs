using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IHotbarUI
{
    public ICollection<IHotbarSlot> Slots { get; }
    public void SetHotbar(ICollection<IHotbarDisplayable> equippedItems);
    public void UpdateSlot(IHotbarDisplayable displayables, int slotIndex);
    public void SelectSlot(int slotIndex);
    public void AddSlot(IHotbarDisplayable displayable);
    public void RemoveSlot(int slotIndex);
    public void OnNoEquippable();
}

public class HotbarUI : MonoBehaviour, IHotbarUI
{
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform hotbarSlots;
    [SerializeField] TextMeshProUGUI itemNameIndicator;
    List<IHotbarSlot> slots;
    int i = 0;

    public ICollection<IHotbarSlot> Slots => slots;

    // Start is called before the first frame update
    void Awake()
    {
        slots = new List<IHotbarSlot>();
    }

    public void SelectSlot(int slotIndex)
    {
        slots[i].OnDeselect();
        i = slotIndex;
        slots[i].OnSelect(itemNameIndicator);
    }

    public void SetHotbar(ICollection<IHotbarDisplayable> equippedItems)
    {
        if (slots.Count != 0)
        {
            foreach (IHotbarSlot slot in slots)
            {
                DestroySlot(slot);
            }

            slots.Clear();
        }

        foreach (IHotbarDisplayable displayable in equippedItems)
        {
            AddSlot(displayable);
        }
    }

    public void UpdateSlot(IHotbarDisplayable item, int slotIndex)
    {
        throw new System.NotImplementedException();
    }

    public void AddSlot(IHotbarDisplayable displayable)
    {
        GameObject newSlot = Instantiate(slotPrefab, hotbarSlots);
        IHotbarSlot slot = newSlot.GetComponent<IHotbarSlot>();
        slots.Add(slot);
        slot.OnEquip(displayable);
    }

    public void RemoveSlot(int slotIndex)
    {
        IHotbarSlot removedSlot = slots[slotIndex];
        slots.Remove(removedSlot);
        DestroySlot(removedSlot);
    }

    private void DestroySlot(IHotbarSlot removedSlot)
    {
        removedSlot.OnDeselect();
        Destroy(removedSlot.gameObject);
    }

    public void OnNoEquippable()
    {
        throw new System.NotImplementedException();
    }
}
