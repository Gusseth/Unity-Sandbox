using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : ActorBase, IKegareAbleActor, IFactionable, IHaveInventory
{
    [SerializeField] IInventoryController inventoryController;
    [SerializeField] ISet<WorldFaction> factions;
    [SerializeField] bool kegare;
    [SerializeField] int kegareTimeout;
    [SerializeField] uint regenFrameInterval = 2;
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
        if (ke >= 0)
        {
            if (!kegare || bypassKegare)
            {
                base.AddKe(ke, showDecrease);
            }
        } 
        else
        {
            base.AddKe(ke, showDecrease);
            if (kegare)
            {
                Debug.Log("Step it up.");
                Kill();
            }
            else if (this.ke == 0)
            {
                OnKegare();
            }
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
        kegare = true;
        Debug.Log("Your soul has been tainted by impurity.");
        if (kegareStack == 0)
            actualHarae = haraeMult;
        haraeMult = 0;
        kegareStack++;
        TimeHelpers.InvokeAsync(KegareTimeout, kegareTimeout, this.GetCancellationTokenOnDestroy());
    }

    private void KegareTimeout()
    {
        if (Alive)
        {
            RemoveKegare(false);
        }
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
            haraeMult = actualHarae / (1 << math.min(kegareStack, 31));
        }
        kegare = false;
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
    void FixedUpdate()
    {
        if (Time.frameCount % regenFrameInterval == 0)
            if (!kegare && regenerateKe)
                AddKe(math.max((int)math.round(harae * haraeMult), 1));
    }

    void OnDifficultyChange(in Difficulty difficulty)
    {
        instantDeath = difficulty.InstantDeath;
    }

    void OnEnable()
    {
        instantDeath = Root.Instance.Difficulty.InstantDeath;
        Root.DifficultyChangeEvent += OnDifficultyChange;
    }

    void OnDisable()
    {
        Root.DifficultyChangeEvent -= OnDifficultyChange;
    }
}
