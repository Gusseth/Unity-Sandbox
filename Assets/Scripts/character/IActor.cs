using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ActorFaction
{
    Player =       0b1,
    Allies =      0b10,
    Neutral =    0b100,
    Enemies =   0b1000
}

public enum WorldFaction
{
    None
}

public class DeathData
{
    public HitData hitData;
    public IActor killer;
    public IActor victim;
    public string deathMessage;
}

/// <summary>
/// Represents everything that can be alive
/// </summary>
public interface IActor
{
    public string Name { get; }
    public bool Alive { get; set; }
    public ActorFaction ActorFaction { get; set; }
    public void OnDeath(DeathData data);
}

public interface IHaveHKS
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int HealthBaseRegen { get; set; }
    public float HealthRegenMult { get; set; }
    public bool Invulnerable { get; set; }

    public int Ke { get; set; }
    public int MaxKe { get; set; }
    public int Harae { get; set; }
    public float HaraeMult { get; set; }
    public bool KamiMode { get; set; }

    public int Stamina { get; set; }
    public int MaxStamina { get; set; }
    public int StaminaBaseRegen { get; set; }
    public float StaminaRegenMult { get; set; }
    public bool InfiniteStamina { get; set; }
}

public interface IDamagableActor
{
    public void AddDamage(HitData data);
    public void AddKe(int ke, bool showDecrease = true, bool bypassKegare = false);
    public void AddStamina(int stamina, bool showDecrease = true);
    public void Kill();
}

public interface IKegareAbleActor
{
    public bool Kegare { get; set; }
    public void OnKegare();
    public void RemoveKegare(bool purification);
}

public interface IFactionable
{
    public ISet<WorldFaction> Factions { get; }
    public void AddFaction(WorldFaction faction);
    public void RemoveFaction(WorldFaction faction);
}

public interface IHaveInventory
{
    IInventoryController InventoryController { get; }
}
