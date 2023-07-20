using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Hit
{
    public int damage;
    public float3 point;
    public float3 normal;
    public IHurtBox hurtBox;
    public IHitterBox hitterBox;

    public bool IsValid()
    {
        if (hurtBox != null)
            if (hurtBox.CheckHit(this))
                return true;
        return false;
    }
}

public interface IHitCheck
{
    public bool CheckHit(Hit data);
}

public interface IHitResponse
{
    public void Response(Hit data);
}

public interface IHitter : IHitCheck, IHitResponse
{
    int damage { get; }
}

public interface IHitterBox : IHitCheck
{
    public bool blocking { get; }
    public bool parry { get; }
    public IHitter hitter { get; set; }
}

public interface IGotHit : IHitCheck, IHitResponse
{

}

public interface IHurtBox : IHitCheck
{
    public bool active { get; }
    public GameObject owner { get; }
    public Transform transform { get; }
    public IGotHit hurtResponder { get; set; }
}