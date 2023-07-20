using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HitterBox : MonoBehaviour, IHitterBox
{
    [SerializeField] new BoxCollider collider;   //TODO: refactor to use generic colliders
    [SerializeField] LayerMask layerMask;
    [SerializeField] bool isBlocking;
    [SerializeField] bool isParrying;

    public bool blocking { get => isBlocking;}
    public bool parry { get => isParrying;}

    private float thickness = 0.025f;

    public IHitter hitter { get; set; }

    public bool CheckHit(Hit data)
    {
        float3 size = math.mul(collider.size, transform.lossyScale);
        float distance = size.y - thickness;
        float3 direction = transform.up;
        float3 center = math.transform(transform.localToWorldMatrix, collider.center);
        float3 start = center - direction * (distance / 2);
        float3 halfExtents = new float3(size.x, thickness, size.z) / 2;

        RaycastHit[] hits = Physics.BoxCastAll(start, halfExtents, direction, transform.rotation, distance, layerMask);

        ProcessHits(hits);

        return true;
    }

    private void ProcessHits(RaycastHit[] hits)
    {
        foreach (RaycastHit hit in hits)
        {
            // If it touches a hurtbox (someone gets hurt!)
            if (hit.collider.TryGetComponent(out IHurtBox hurtBox))
            {
                if (!hurtBox.active) continue;

                Hit data = new Hit
                {
                    damage = hitter.damage,
                    point = hit.point == Vector3.zero ? collider.center : hit.point,
                    normal = hit.normal,
                    hurtBox = hurtBox,
                    hitterBox = this
                };

                if (hurtBox.CheckHit(data))
                {
                    data.hitterBox.hitter.Response(data);
                    data.hurtBox.hurtResponder.Response(data);
                }
            }

            // If it touches another hitterbox (you got blocked lmao)
            else if (hit.collider.TryGetComponent(out IHitterBox hitterBox)) {
                if (hitterBox.blocking)
                {
                    OnBlocked();
                    Debug.Log("I hit a HitterBox, meaning I got blocked :(");
                }
                else if (hitterBox.parry)
                {
                    OnParried();
                    Debug.Log("Massive L");
                }
            }
        }
    }

    void OnBlocked()
    {
        // TODO: Add a melee cancel here
    }

    void OnParried()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        collider ??= GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
