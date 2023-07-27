using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ActorBase, IFactionable, IHaveInventory
{
    [SerializeField] IInventoryController inventoryController;
    [SerializeField] ISet<WorldFaction> factions;

    // You WILL only be the player whether you like it or not
    public override ActorFaction ActorFaction { get => ActorFaction.Player; set => actorFaction = ActorFaction.Player; }

    public ISet<WorldFaction> Factions { get => factions; }
    public IInventoryController InventoryController => inventoryController;

    public void AddFaction(WorldFaction faction)
    {
        Factions.Add(faction);
    }

    public void RemoveFaction(WorldFaction faction)
    {
        Factions.Remove(faction);
    }

    // Start is called before the first frame update
    void Start()
    {
        factions = new HashSet<WorldFaction>();
        actorFaction = ActorFaction.Player;
        actorName = "Player";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
