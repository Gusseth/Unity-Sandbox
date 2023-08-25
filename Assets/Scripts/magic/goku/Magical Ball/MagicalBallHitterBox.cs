using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MagicalBallHitterBox : MonoBehaviour, IHitterBox
{
    [SerializeField] GameObject owner;
    [SerializeField] public AbstractActorBase gokuOwner;
    [SerializeField] SphereCollider actualCollider;
    [SerializeField] LayerMask layerMask;
    [SerializeField] HitBoxLayer hitBoxLayer;
    [SerializeField] float forceStrength = 0;

    IHitter hitter;
    bool hasHit = false;
    HitBoxFaction faction;
    ISet<Collider> alreadyHitColliders; // don't need to clear because this will get destroyed anyways

    public IHitter Hitter { get => hitter; set => hitter = value; }

    public GameObject Owner => owner;

    public HitBoxLayer HitBoxLayer => hitBoxLayer;

    public void Attack()
    {
        //float distance = distance_gizmo = size.y;
        float radius = actualCollider.radius * math.cmax(math.abs(transform.lossyScale));
        float3 direction = transform.forward;
        float3 origin = math.transform(transform.localToWorldMatrix, actualCollider.center);
        //float3 halfExtents = size_gizmo = size / 2;

        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, direction, 0, layerMask);

        ProcessHits(hits, origin);
    }

    private void ProcessHits(RaycastHit[] hits, float3 origin)
    {
        foreach (RaycastHit hit in hits)
        {
            // We're detecting ourselves lol
            if (hit.collider == actualCollider) continue;
            if (alreadyHitColliders.Contains(hit.collider)) continue;

            // I fucking hate Unity for making colliders that overlap in the first
            // sweep return a hit.point of (0, 0, 0) and a hit.normal of -direction.
            float3 point = hit.collider.ClosestPoint(origin);
            float3 normal = new float3(origin - point);
            normal = math.normalize(normal);

            Root.Debug.DrawPointNormals(new Tuple<float3, float3>(point, normal));

            if (hit.collider.TryGetComponent(out ICheckHitLayer hitLayerObject))
            {
                if (!MathHelpers.FlagContains((byte)HitBoxLayer, (byte)hitLayerObject.HitBoxLayer))
                    continue;

                if (hitLayerObject is IHurtBox hurtBox)
                {
                    if (!hurtBox.Active) continue;
                    if (gokuOwner != null && hurtBox.Owner == gokuOwner.gameObject) continue;

                    HitData data = new Hit(
                        Hitter.Damage,
                        point,
                        normal,
                        hurtBox,
                        this
                        );


                    OnHurtBoxHit(hurtBox, data);
                    hasHit = true;
                }
                else if (hitLayerObject is IBlocker blocker && blocker.Blocking)
                {
                    hasHit = true;
                    Block data = new(Hitter.Damage, point, normal, blocker.Parry, 0, this, blocker);
                    OnBlockerHit(blocker, data);
                    break;
                }
            }
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Hitbox"))
            {
                if (hit.collider.TryGetComponent(out Rigidbody rb))
                {
                    Vector3 explosiveForce = -normal * forceStrength;

                    rb.AddForceAtPosition(explosiveForce, point, ForceMode.Impulse);
                }

                hasHit = true;
            }

            alreadyHitColliders.Add(hit.collider);
        }

        if (hasHit)
            hitter.PostAttack();
    }

    private void OnBlockerHit(IBlocker blocker, Block data)
    {
        if (blocker.CheckHit(data))
        {
            data.attacker.Hitter.Response(data);
            data.blocker.Response(data);
        }
    }

    public void OnHurtBoxHit(IHurtBox hurtBox, HitData data)
    {
        Hit hitData = (Hit)data;
        if (hurtBox.CheckHit(hitData))
        {
            hitData.hitterBox.Hitter.Response(data);
            hitData.hurtBox.HurtResponder.Response(data);
        }
    }

    public void UpdateValues(AbstractActorBase actor)
    {
        if (actor != null)
        {
            gokuOwner = actor;
            faction = AbstractActorBase.ActorToHitBoxFaction(actor.ActorFaction);
        }
        faction = HitBoxFaction.Neutral;
    }

    // Start is called before the first frame update
    void Awake()
    {
        alreadyHitColliders = new HashSet<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        
    }

    public void PreAttack(IHitter hitter)
    {
        
    }

    public void PostAttack(IHitter hitter)
    {
        
    }
}
