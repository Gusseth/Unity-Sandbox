using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ActorBase : MonoBehaviour, IActor, IHaveHKS
{
    [SerializeField] protected string actorName = "Actor";
    [SerializeField] protected bool alive = true;
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected bool invulnerable = false;
    [SerializeField] protected int ke;
    [SerializeField] protected int maxKe;
    [SerializeField] protected bool kamiMode = false;
    [SerializeField] protected int stamina;
    [SerializeField] protected int maxStamina;
    [SerializeField] protected bool infiniteStamina = false;
    [SerializeField] protected ActorFaction actorFaction;

    public string Name { get => actorName; }
    public bool Alive { get => alive; set => alive = value; }
    public bool Invulnerable { get => invulnerable; set => invulnerable = value; }
    public int Health { get => health; set => AddDamage(health - value); }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }

    // Recall that Ke = mana
    public int Ke { get => ke; set => ke = value; }
    public int MaxKe { get => maxKe; set => maxKe = value; }
    public bool KamiMode { get => kamiMode; set => kamiMode = value; }

    public int Stamina { get => stamina; set => stamina = value; }
    public int MaxStamina { get => maxStamina; set => maxStamina = value; }
    public bool InfiniteStamina { get => infiniteStamina; set => infiniteStamina = value; }

    // You cannot be anything else but the player!!
    public ActorFaction ActorFaction { get => actorFaction; set => actorFaction = value; }

    protected void AddDamage(int damage)
    {
        HitData data = new Hit(damage);
        AddDamage(data);
    }

    public void AddDamage(HitData data)
    {
        int damage = data.damage;

        if (alive)
        {
            if (damage > health)
            {
                IActor killer = null;
                if (data.agressor != null)
                    data.agressor.GetComponent<IActor>();

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
    }

    public void Kill()
    {
        AddDamage(health + 1);
    }

    public void OnDeath(DeathData data)
    {
        alive = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
