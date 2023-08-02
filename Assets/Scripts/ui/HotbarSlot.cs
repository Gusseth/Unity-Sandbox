using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public interface IHotbarSlot
{
    public GameObject gameObject { get; }
    public ItemStack Item { get; }
    public void OnSelect(TextMeshProUGUI itemNameIndicator);
    public void OnDeselect();
    public void OnEquip(ItemStack item);
    public void OnUnequip();
}

public class HotbarSlot : MonoBehaviour, IHotbarSlot
{
    [SerializeField] RectTransform rTransform;
    [SerializeField] float2 sizeSelected;
    [SerializeField] float2 sizeUnSelected;
    [SerializeField] ItemStack item;

    public ItemStack Item => item;

    public void OnSelect(TextMeshProUGUI itemNameIndicator)
    {
        rTransform.sizeDelta = sizeSelected;
        itemNameIndicator.text = item.Name;
    }

    public void OnDeselect()
    {
        rTransform.sizeDelta = sizeUnSelected;
    }

    public void OnEquip(ItemStack item)
    {
        this.item = item;
    }

    public void OnUnequip()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        rTransform ??= GetComponent<RectTransform>();
        sizeSelected = new float2(100, 100);
        sizeUnSelected = new float2(75, 75);
    }
}
