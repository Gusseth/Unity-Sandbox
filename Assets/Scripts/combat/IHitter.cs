using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

/*
 * Thank you to HamJoy Games for this system. You can find the tutorial here:
 * https://www.youtube.com/watch?v=TM9tADSh9nE
 * 
 * This uses a heavily modified version of the system in the video.
 */

public abstract class HitData
{
    public int damage;
    public float3 point;
    public float3 normal;
    public GameObject aggressor;
    public GameObject victim;

    public abstract bool IsValid();
}

public class Hit : HitData
{
    public IHurtBox hurtBox;
    public IHitterBox hitterBox;

    public Hit(int damage, float3 point, float3 normal, IHurtBox hurtBox, IHitterBox hitterBox)
    {
        this.damage = damage;
        this.point = point;
        this.normal = normal;
        this.hurtBox = hurtBox;
        this.hitterBox = hitterBox;
        aggressor = hitterBox.Owner;
        victim = hurtBox.Owner;
    }

    public Hit(float3 point, float3 normal, IHitterBox hitterBox, IHurtBox hurtBox)
    {
        this.damage = hitterBox.Hitter.Damage;
        this.point = point;
        this.normal = normal;
        this.hurtBox = hurtBox;
        this.hitterBox = hitterBox;
        aggressor = hitterBox.Owner;
        victim = hurtBox.Owner;
    }

    /*
     *  Do not use this: only for debugging purposes!
     */
    public Hit(int damage)
    {
        this.damage = damage;
    }

    public override bool IsValid()
    {
        if (hurtBox != null)
            if (hurtBox.CheckHit(this))
                return true;
        return false;
    }
}

public class Block : HitData
{
    public bool parry;
    public float force;
    public IHitterBox attacker;
    public IBlocker blocker;

    public Block(int damage, float3 point, float3 normal, bool parry, float force, IHitterBox attacker, IBlocker blocker)
    {
        this.damage = damage;
        this.point = point;
        this.normal = normal;
        this.parry = parry;
        this.force = force;
        this.attacker = attacker;
        this.blocker = blocker;
        aggressor = attacker.Owner;
        victim = blocker.Owner;
    }

    public Block(float3 point, float3 normal, float force, IHitterBox attacker, IBlocker blocker)
    {
        this.damage = attacker.Hitter.Damage;
        this.point = point;
        this.normal = normal;
        this.parry = blocker.Parry;
        this.force = force;
        this.attacker = attacker;
        this.blocker = blocker;
        aggressor = attacker.Owner;
        victim = blocker.Owner;
    }

    public override bool IsValid()
    {
        if (attacker != null)
                return true;
        return false;
    }
}

[System.Flags]
public enum HitBoxFaction
{
    Player  =    0b1,
    Allies  =   0b10,
    Neutral =  0b100,
    Enemies = 0b1000
}

[System.Flags]
public enum HitBoxLayer
{
    Physical    =     0b1,
    Magical     =    0b10,
    Divine      =   0b100
}

public enum HitBoxType
{
    // Order is from most important/damaging to least
    Fatal,
    Critical,
    Head,
    WeakSpot,
    Torso,
    Limb,
    NoDamage
}

public enum BasicHitDirection
{
    // Order is from most important/damaging to least
    None,
    Up,
    Down,
    Left,
    Right
}

/// <summary>
/// Used to implement hit validation
/// </summary>
public interface IHitCheck
{
    /// <summary>
    /// Validates receiving hits to determine if the hit is legal
    /// </summary>
    /// <param name="data">Contains information about the hit</param>
    /// <returns><c>True</c> if the hit is legal,
    /// <c>False</c> if not</returns>
    public bool CheckHit(HitData data);
}

public interface IHitLayerObject : IGiveOwnerMetadata
{
    public HitBoxLayer HitBoxLayer { get; }
}

/// <summary>
/// Used to implement what happens after a successful hit
/// </summary>
public interface IHitResponse
{
    /// <summary>
    /// Called after the weapon successfully hits the target or
    /// after a receiving hit is validated
    /// </summary>
    /// <param name="data">Contains information about the hit</param>
    public void Response(HitData data);
}

public interface IBlocker : IGiveOwnerMetadata, IHitCheck, IHitResponse, IHitLayerObject
{
    /// <summary>
    /// Returns true if the hitter is blocking their weapon
    /// </summary>
    public bool Blocking { get; set; }
    /// <summary>
    /// Returns true if the hitter parried with their weapon
    /// </summary>
    public bool Parry { get; set; }
    /// <summary>
    /// Called immediately after the hitter receives a request to block
    /// </summary>
    public void PreBlock(float3 direction);
    /// <summary>
    /// Called after the hitter returns to non-blocking state
    /// </summary>
    public void PostBlock();
    /// <summary>
    /// Called if this blocker has successfully hit a hitterbox
    /// </summary>
    /// <param name="data">Contains information about the block</param>
    public void OnBlock(Block data);

    /// <summary>
    /// Called if this blocker has successfully parried a hitterbox
    /// </summary>
    /// <param name="data">Contains information about the block</param>
    public void OnParry(Block data);
}

/// <summary>
/// Interface that must be implemented by GameObjects that detects hurtboxes
/// </summary>
public interface IHitter : IHitCheck, IHitResponse, IHitLayerObject, IGiveOwnerMetadata
{
    /// <summary>
    /// Raw damage dealt by the weapon
    /// </summary>
    int Damage { get; }
    /// <summary>
    /// Returns true if the hitter is in the attacking state
    /// </summary>
    public bool Attacking { get; set; }
    /// <summary>
    /// Called immediately after the hitter receives a request to attack
    /// </summary>
    public bool PreAttack(float3 direction, AbstractActorBase actor);
    /// <summary>
    /// Called after the hitter returns to idle state
    /// </summary>
    public void PostAttack();
}

public interface IDirectionalHitter
{
    /// <summary>
    /// Updates attack direction indicators given the direction
    /// </summary>
    /// <param name="deltaVelocity">Direction of recent movement ie. 'direction' from PreAttack</param>
    /// <param name="indicator">The indicator to be modified</param>
    public void UpdateDirectionalIndicator(float3 deltaVelocity, IAttackDirectionalUI indicator);
}

/// <summary>
/// Interface that must be implemented by individual hitboxes.
/// Yes, this includes the hitbox for your sword.
/// </summary>
public interface IHitterBox : IGiveOwnerMetadata, IHitLayerObject
{
    /// <summary>
    /// The hitter linked with this hitbox
    /// </summary>
    public IHitter Hitter { get; set; }
    /// <summary>
    /// Called when the hitbox receives an attack signal from the Hitter
    /// </summary>
    public void Attack();

    public void PreAttack(IHitter hitter);
    public void PostAttack(IHitter hitter);

    //public void OnHitterBoxHit(IHitterBox hitterBox, HitData data);
    //public void OnHurtBoxHit(IHurtBox hurtBox, HitData data);
}

/// <summary>
/// Interface that must be implemented by entities that get hurt
/// </summary>
public interface IHitResponder : IHitCheck, IHitResponse
{

}

/// <summary>
/// Interface that must be implemented by individual hurtboxes
/// </summary>
public interface IHurtBox : IHitCheck, IGiveOwnerMetadata, IHitLayerObject
{
    /// <summary>
    /// Is this hurtbox active for detection right now
    /// </summary>
    public bool Active { get; }
    /// <summary>
    /// The HurtResponder linked to this hurtbox
    /// </summary>
    public IHitResponder HurtResponder { get; set; }
}

public interface IGiveOwnerMetadata
{
    /// <summary>
    /// The root entity of this hurtbox
    /// </summary>
    public GameObject Owner { get; }
    /// <summary>
    /// The hurtbox's transform component
    /// </summary>
    public Transform transform { get; } // blame unity naming scheme
}