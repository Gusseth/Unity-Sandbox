using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemScript : MonoBehaviour, IInteractable
{
    [SerializeField] bool interactable = true;
    [SerializeField] GameObject parentObject;
    [SerializeField] ItemStack item;

    public bool Interactable { get => interactable; set => interactable = value; }

    public HoverData OnHover(RaycastHit hit)
    {
        return new HoverData
        {
            data = item,
            name = item.Name,
            description = item.Description
        };
    }

    public void OnInteract(AbstractActorBase actor)
    {
        if (interactable)
        {
            if (actor.TryGetComponent(out IInventoryController inventory))
            {
                inventory.AddItem(item);
                Destroy(parentObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent == null)
            parentObject ??= gameObject;
        else
            parentObject ??= transform.parent.gameObject;
    }
}
