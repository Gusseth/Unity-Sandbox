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
    public bool Invulnerable { get; set; }

    public int Ke { get; set; }
    public int MaxKe { get; set; }
    public bool KamiMode { get; set; }

    public int Stamina { get; set; }
    public int MaxStamina { get; set; }
    public bool InfiniteStamina { get; set; }

    public void AddDamage(HitData data);
    public void Kill();
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
