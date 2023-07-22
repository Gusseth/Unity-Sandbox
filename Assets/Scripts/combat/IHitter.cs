using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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

    public abstract bool IsValid();
}

public class Hit : HitData
{
    public IHurtBox hurtBox;
    public IHitterBox hitterBox;

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
    public IHitterBox weaponBlocked;
    public IHitterBox blocker;

    public override bool IsValid()
    {
        if (weaponBlocked != null)
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

public interface IBlocker
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
    public void PreBlock();
    /// <summary>
    /// Called after the hitter returns to non-blocking state
    /// </summary>
    public void PostBlock();

}

/// <summary>
/// Interface that must be implemented by GameObjects that detects hurtboxes
/// </summary>
public interface IHitter : IHitCheck, IHitResponse
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
    public void PreAttack();
    /// <summary>
    /// Called after the hitter returns to idle state
    /// </summary>
    public void PostAttack();
}

/// <summary>
/// Basic interface for all melee weapons
/// </summary>
public interface IMeleeHitter : IHitter, IBlocker {

}

/// <summary>
/// Interface that must be implemented by individual hitboxes
/// </summary>
public interface IHitterBox
{
    /// <summary>
    /// 
    /// </summary>
    public IHitter Hitter { get; set; }
    public void Attack();
    //public void OnHitterBoxHit(IHitterBox hitterBox, HitData data);
    //public void OnHurtBoxHit(IHurtBox hurtBox, HitData data);
}

/// <summary>
/// Interface that must be implemented by entities that get hurt
/// </summary>
public interface IGotHit : IHitCheck, IHitResponse
{

}

/// <summary>
/// Interface that must be implemented by individual hurtboxes
/// </summary>
public interface IHurtBox : IHitCheck
{
    public bool Active { get; }
    public GameObject Owner { get; }
    public Transform transform { get; } // blame unity naming scheme
    public IGotHit HurtResponder { get; set; }
}