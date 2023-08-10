using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public interface IHotbarSlot
{
    public GameObject gameObject { get; }
    public IHotbarDisplayable Displayable { get; }
    public void OnSelect(TextMeshProUGUI itemNameIndicator);
    public void OnDeselect();
    public void OnEquip(IHotbarDisplayable item);
    public void OnUnequip();
}

public class HotbarSlot : MonoBehaviour, IHotbarSlot
{
    [SerializeField] RectTransform rTransform;
    [SerializeField] float2 sizeSelected;
    [SerializeField] float2 sizeUnSelected;
    [SerializeField] IHotbarDisplayable displayable;

    public IHotbarDisplayable Displayable => displayable;

    public void OnSelect(TextMeshProUGUI itemNameIndicator)
    {
        rTransform.sizeDelta = sizeSelected;
        itemNameIndicator.text = displayable.HotbarName;
    }

    public void OnDeselect()
    {
        rTransform.sizeDelta = sizeUnSelected;
    }

    public void OnEquip(IHotbarDisplayable displayable)
    {
        this.displayable = displayable;
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
