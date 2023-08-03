using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MeleeHitterBox : MonoBehaviour, IHitterBox
{
    [SerializeField] GameObject owner;
    [SerializeField] new BoxCollider collider;   //TODO: refactor to use generic colliders
    [SerializeField] LayerMask layerMask;
    [SerializeField] int verticalSubdivisions = 8;  // Works best with powers of 2
    [SerializeField] GameObject spark;
    public ISet<Collider> alreadyHitColliders { get; private set; }

    private float thickness = 0.025f;   // internal var, do not touch

    public IHitter Hitter { get; set; }

    public GameObject Owner => owner;

    public void Attack()
    {
        float3 size = collider.size * (float3)transform.lossyScale;

        /*
         * Subdividing a box collider may seem overengineered but it's because of the way 
         * Unity calculates normals and collision position. If we only make a box the size of
         * the collider, then unity returns a (0, -1, 0) normal vector and (0, 0, 0) collision point
         * 
         * Blame Unity devs.
         */

        float distance = size.y - thickness;
        //float distance = distance_gizmo = size.y;
        float3 direction = transform.up;
        float3 center = math.transform(transform.localToWorldMatrix, collider.center);
        float3 start = center - direction * (distance / 2);
        //float3 start = center_gizmo = center;
        float3 halfExtents = new float3(size.x, thickness, size.z) / 2;
        //float3 halfExtents = size_gizmo = size / 2;

        RaycastHit[] hits = Physics.BoxCastAll(start, halfExtents, direction, transform.rotation, distance, layerMask);

        ProcessHits(hits);
    }

    private void ProcessHits(RaycastHit[] hits)
    {
        foreach (RaycastHit hit in hits)
        {
            // We're detecting ourselves lol
            if (hit.collider == collider) continue;
            if (alreadyHitColliders.Contains(hit.collider)) continue;

            alreadyHitColliders.Add(hit.collider);

            float3 point = hit.point;
            if (point.Equals(float3.zero))
            {
                point = math.transform(transform.localToWorldMatrix, collider.center);
            }

            Root.Debug.DrawPointNormals(new Tuple<float3, float3>(point, hit.normal));
            

            // If it touches another hitterbox (you got blocked lmao)
            if (hit.collider.TryGetComponent(out IHitterBox hitterBox))
            {
                IBlocker blocker = hitterBox.Hitter as IBlocker;
                if (blocker == null) continue;

                Block data = new Block(
                    Hitter.Damage,
                    point,
                    hit.normal,
                    blocker.Parry,
                    0,
                    this,
                    hitterBox
                    );


                if (blocker.Blocking)
                {
                    OnBlocked(data);
                }
                else if (blocker.Parry)
                {
                    OnParried(data);
                }
                break;
            }
            // If it touches a hurtbox (someone gets hurt!)
            else if (hit.collider.TryGetComponent(out IHurtBox hurtBox))
            {
                if (!hurtBox.Active) continue;

                HitData data = new Hit(
                    Hitter.Damage,
                    point,
                    hit.normal,
                    hurtBox,
                    this
                    );


                OnHurtBoxHit(hurtBox, data);
            }
        }
    }

    void OnBlocked(Block blockData)
    {
        // TODO: Add a melee cancel here
        Hitter.Attacking = false;
        Debug.Log("I hit a HitterBox, meaning I got blocked :(");
        GameObject s = Instantiate(spark);

        s.transform.position = blockData.point + blockData.normal * .05f;
        s.transform.Rotate(blockData.normal);

        Root.Debug.DrawLight(s.transform.position);
        Hitter.PostAttack();
    }

    void OnParried(Block blockData)
    {
        Debug.Log("Massive L");
        OnBlocked(blockData);
    }

    // Start is called before the first frame update
    void Start()
    {
        collider ??= GetComponent<BoxCollider>();
        alreadyHitColliders = new HashSet<Collider>();
        thickness = (collider.size * (float3)transform.lossyScale).y / verticalSubdivisions;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHitterBoxHit(IHitterBox hitterBox, HitData data)
    {
        IBlocker blocker = (IBlocker)hitterBox.Hitter;
        Block blockData = (Block)data;
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
}
