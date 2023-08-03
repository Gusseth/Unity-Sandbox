using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Basic implementation of AbstractActorBase
/// </summary>
public class ActorBase : AbstractActorBase
{
    public override string Name { get => actorName; set => actorName = value; }
    public override bool Alive { get => alive; set => alive = value; }

    public override int Health { get => health; set => AddDamage(health - value); }
    public override int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public override int HealthBaseRegen { get => healthBaseRegen; set => healthBaseRegen = value; }
    public override float HealthRegenMult { get => healthRegenMult; set => healthRegenMult = value; }
    public override bool Invulnerable { get => invulnerable; set => invulnerable = value; }

    // Recall that Ke = mana
    public override int Ke { get => ke; set => ke = value; }
    public override int MaxKe { get => maxKe; set => maxKe = value; }
    public override int Harae { get => harae; set => harae = value; }
    public override float HaraeMult { get => haraeMult; set => haraeMult = value; }
    public override bool KamiMode { get => kamiMode; set => kamiMode = value; }  // unlimited mana

    public override int Stamina { get => stamina; set => stamina = value; }
    public override int MaxStamina { get => maxStamina; set => maxStamina = value; }
    public override int StaminaBaseRegen { get => staminaBaseRegen; set => staminaBaseRegen = value; }
    public override float StaminaRegenMult { get => staminaRegenMult; set => staminaRegenMult = value; }
    public override bool InfiniteStamina { get => infiniteStamina; set => infiniteStamina = value; }

    public override ActorFaction ActorFaction { get => actorFaction; set => actorFaction = value; }

    public override float Reach { get => reach; set => reach = value; }

    [DoNotSerialize]
    private UIBarScript barHealth;

    protected override void AddDamage(int damage)
    {
        HitData data = new Hit(damage);
        AddDamage(data);
    }

    public override void AddDamage(HitData data)
    {
        int damage = data.damage;

        if (invulnerable)
        {

        }
        else if (alive)
        {
            if (damage > health)
            {
                health = 0;

                IActor killer = null;
                if (data.aggressor != null)
                    data.aggressor.GetComponent<IActor>();

                DeathData deathData = new DeathData
                {
                    deathMessage = "died",
                    victim = this,
                    killer = killer,
                    hitData = data
                };

                OnDeath(deathData);
            }
            else
            {
                health = math.min(health - damage, maxHealth);
            }
        }

        UpdateHKSBars();
    }

    protected override void UpdateHKSBars()
    {
        if (barHealth != null)
        {
            barHealth.UpdateTarget(health, maxHealth);
        }
    }

    public override void Kill()
    {
        AddDamage(health + 1);
    }

    public override void OnDeath(DeathData data)
    {
        alive = false;
    }

    private void Start()
    {
        barHealth = GetComponentInChildren<UIBarScript>();
        if (barHealth != null)
        {
            barHealth.UpdateTarget(health, maxHealth);
        }
    }
}
