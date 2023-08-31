using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MagicalBallHitter : MonoBehaviour, IHitter
{
    [SerializeField] int damage;
    [SerializeField] bool isAttacking = true;
    [SerializeField] bool isDirectional = false;
    [SerializeField] MagicalBallHitterBox hitterBox;
    [SerializeField] HitBoxLayer hitBoxLayer;
    public int Damage => damage;

    public bool Attacking { get => isAttacking; set => isAttacking = value; }
    public GameObject Owner => gameObject;

    public HitBoxLayer HitBoxLayer => hitBoxLayer;

    public bool CheckHit(HitData data)
    {
        return true;
    }

    public void PostAttack()
    {
        Destroy(gameObject);
    }

    public bool PreAttack(float3 direction, AbstractActorBase actor)
    {
        hitterBox.UpdateValues(actor);
        return true;
    }

    public void Response(HitData data)
    {
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        hitterBox.Hitter = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {
            hitterBox.Attack();
        }
    }
}
