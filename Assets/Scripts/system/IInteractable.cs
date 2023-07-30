using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverData
{
    public IInteractableData data;
    public string name;
    public string description;
    public uint amount;
}

public interface IInteractable
{
    public bool Interactable { get; set; }
    public void OnInteract(AbstractActorBase actor);
    public HoverData OnHover(RaycastHit hit);
}

public interface IInteractableData
{
    public string Name { get; }
    public string Description { get; }
    public uint Amount { get; }
}
