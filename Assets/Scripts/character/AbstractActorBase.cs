using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents ALL NPCs, enemies, players, anything alive that has Health, Ke and Stamina
/// </summary>
[Serializable]
public abstract class AbstractActorBase : MonoBehaviour, IActor, IHaveHKS, IDamagableActor, ICheckExclusives
{
    [SerializeField] protected string actorName = "Actor";
    [SerializeField] protected bool alive = true;
    [SerializeField] protected int health;
    [SerializeField] protected int healthBaseRegen;
    [SerializeField] protected float healthRegenMult = 1;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected bool regenerateHealth = true;
    [SerializeField] protected bool invulnerable = false;

    [SerializeField] protected int ke;
    [SerializeField] protected int harae;
    [SerializeField] protected float haraeMult = 1;
    [SerializeField] protected int maxKe;
    [SerializeField] protected bool regenerateKe = true;
    [SerializeField] protected bool kamiMode = false;

    [SerializeField] protected int stamina;
    [SerializeField] protected int maxStamina;
    [SerializeField] protected int staminaBaseRegen;
    [SerializeField] protected float staminaRegenMult = 1;
    [SerializeField] protected bool regenerateStamina = true;
    [SerializeField] protected bool infiniteStamina = false;
    [SerializeField] protected ActorFaction actorFaction;

    [SerializeField] protected float reach = 2;

    protected ISet<object> excludables;
    protected bool instantDeath = false;

    public abstract string Name { get; set; }
    public abstract bool Alive { get; set; }

    public abstract int Health { get; set; }
    public abstract int MaxHealth { get; set; }
    public abstract int HealthBaseRegen { get; set; }
    public abstract float HealthRegenMult { get; set; }
    public abstract bool RegenerateHealth { get; set; }
    public abstract bool Invulnerable { get; set; }

    // Recall that Ke = mana
    public abstract int Ke { get; set; }
    public abstract int MaxKe { get; set; }
    public abstract int Harae { get; set; }
    public abstract float HaraeMult { get; set; }
    public abstract bool RegenerateKe { get; set; }
    public abstract bool KamiMode { get; set; }  // unlimited mana

    public abstract int Stamina { get; set; }
    public abstract int MaxStamina { get; set; }
    public abstract int StaminaBaseRegen { get; set; }
    public abstract float StaminaRegenMult { get; set; }
    public abstract bool RegenerateStamina { get; set; }
    public abstract bool InfiniteStamina { get; set; }

    public abstract ActorFaction ActorFaction { get; set; }

    public abstract float Reach { get; set; }

    protected abstract void AddDamage(int damage);
    public abstract void AddDamage(HitData data);

    public abstract void AddKe(int ke, bool showDecrease = true, bool bypassKegare = false);
    public abstract void AddKe(float ke, bool showDecrease = true, bool bypassKegare = false);
    public abstract void AddStamina(int stamina, bool showDecrease = true);
    public abstract void AddStamina(float stamina, bool showDecrease = true);

    public abstract void Kill();
    protected abstract void UpdateHKSBars();
    public abstract void OnDeath(DeathData data);

    public virtual void AddExcludable(object excludable)
    {
        excludables ??= new HashSet<object>();
        excludables.Add(excludable);
    }

    public virtual void DeleteExcludable(object excludable)
    {
        if (excludables == null) return;
        excludables.Remove(excludable);
    }

    public virtual bool ExcludableExists(object excludable)
    {
        if (excludables == null) return false;
        return excludables.Contains(excludable);
    }

    public static HitBoxFaction ActorToHitBoxFaction(ActorFaction faction)
    {
        switch (faction)
        {
            case ActorFaction.Player:
                return HitBoxFaction.Player;
            case ActorFaction.Neutral:
                return HitBoxFaction.Neutral;
            default:
                return HitBoxFaction.Enemies;
        }
    }

    public override string ToString() => Name;
}
