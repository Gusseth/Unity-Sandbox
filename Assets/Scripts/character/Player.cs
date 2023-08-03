using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ActorBase, IKegareAbleActor, IFactionable, IHaveInventory
{
    [SerializeField] IInventoryController inventoryController;
    [SerializeField] ISet<WorldFaction> factions;
    [SerializeField] bool kegare;
    [SerializeField] UIBarScript healthBar;
    [SerializeField] UIBarScript keBar;
    [SerializeField] UIBarScript staminaBar;
    float actualHarae;
    int kegareStack;

    // You WILL only be the player whether you like it or not
    public override ActorFaction ActorFaction { get => ActorFaction.Player; set => actorFaction = ActorFaction.Player; }

    public ISet<WorldFaction> Factions { get => factions; }
    public IInventoryController InventoryController => inventoryController;
    public bool Kegare { get => kegare; set => kegare = value; }

    public void AddFaction(WorldFaction faction)
    {
        Factions.Add(faction);
    }

    public override void AddKe(int ke, bool showDecrease = true, bool bypassKegare = false)
    {
        if (ke > 0)
        {
            if (!kegare || bypassKegare)
            {
                base.AddKe(ke, showDecrease);
            }
        } 
        else
        {
            base.AddKe(ke, showDecrease);
        }

        keBar.UpdateTarget(this.ke, maxKe, showDecrease);
    }

    public override void AddStamina(int stamina, bool showDecrease = true)
    {
        base.AddStamina(stamina, showDecrease);

        staminaBar.UpdateTarget(this.stamina, maxStamina, showDecrease);
    }

    protected override void UpdateHKSBars()
    {
        healthBar.UpdateTarget(health, maxHealth);
        keBar.UpdateTarget(ke, maxKe);
        staminaBar.UpdateTarget(stamina, maxStamina);
    }

    public void RemoveFaction(WorldFaction faction)
    {
        Factions.Remove(faction);
    }

    public void OnKegare()
    {
        Debug.Log("Your soul has been tainted by impurity.");
        actualHarae = haraeMult;
        haraeMult = 0;
        kegareStack++;
    }

    public void RemoveKegare(bool purification)
    {
        if (purification)
        {
            Debug.Log("Your soul has been purified.");
            kegareStack = 0;
            haraeMult = actualHarae;
        } 
        else
        {
            Debug.Log("Your soul still remains weak but your will lets you fight.");
            haraeMult = actualHarae / (1 + kegareStack);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        factions = new HashSet<WorldFaction>();
        actorFaction = ActorFaction.Player;
        actorName = "Player";
        UpdateHKSBars();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
