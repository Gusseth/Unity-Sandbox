using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class HandsController : MonoBehaviour
{
    [SerializeField] GameObject LeftHand;
    [SerializeField] GameObject RightHand;
    //[SerializeField] List<GameObject> Hands;
    [SerializeField] GameObject Ball;
    [SerializeField] new Camera camera;
    [SerializeField] BetterCameraController cameraController;
    [SerializeField] uint maxRayDistance;
    [SerializeField] float ballSpeed;
    [SerializeField] IEquipMetadata equipped;
    [SerializeField] AttackDirectionUI directionIndicator;
    IInventoryController inventoryController;
    INoritoInventoryController noritoController;
    AbstractActorBase actor;
    LayerMask rayLayer;

    // TRS Matrix to transfer from Player space to Camera space
    float4x4 T_pc;
    IHitter hitter;

    // Start is called before the first frame update
    void Awake()
    {
        BuildT_pcMatrix();
        rayLayer = LayerMask.NameToLayer("Default");
    }

    void Start()
    {
        cameraController ??= GetComponent<BetterCameraController>();
        inventoryController ??= GetComponent<IInventoryController>();
        actor ??= GetComponent<AbstractActorBase>();

        if (inventoryController is INoritoInventoryController nic)
            noritoController = nic;
        else
            noritoController ??= GetComponent<INoritoInventoryController>();

        GameObject temp = inventoryController.SetEquipped(0, GetHand(Hand.Right).transform);
        hitter = temp.GetComponent<IHitter>();
        equipped = temp.GetComponent<IEquipMetadata>();
        equipped.OnEquip(inventoryController, noritoController, actor);
    }

    private void Update()
    {
        if (hitter != null && hitter.IsDirectional)
        {
            hitter.UpdateDirectionalIndicator(GetAttackDirection(), directionIndicator);
        }
    }

    public void OnRightHand(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (equipped.EquippableType == EquippableType.weaponMagic)
            {
                CastMagic(context);
            }
        }
        else if (context.canceled)
        {
            float3 attackDirection = GetAttackDirection();

            if (equipped.EquippableType == EquippableType.weaponMagic)
            {
                CastMagic(context);
            }
            else
            {
                if (hitter.Attacking)
                    hitter.PostAttack();    // temporary branch, will do it automatically in the future
                else
                    hitter.PreAttack(attackDirection, actor);
            }
        }
    }

    private void CastMagic(InputAction.CallbackContext inputContext)
    {
        CastingData data = ConstructCastingData(inputContext);
        noritoController.OnCast(data);
    }

    private CastingData ConstructCastingData(InputAction.CallbackContext inputContext)
    {
        float3 direction = CalculateFocalVector(GetHand(Hand.Right).transform);
        RaycastHit hit = RaycastFromCamera(maxRayDistance);
        CastingData data = new CastingData
        {
            owner = actor.gameObject,
            ownerActor = actor,

            point = hit.point,
            distance = hit.distance,
            normal = hit.normal,

            origin = RightHand.transform,
            speed = ballSpeed,
            direction = direction,
            directionFunction = CalculateFocalVector,

            inputFlags = MathHelpers.BoolToFlag8(bit0: inputContext.started, bit1: inputContext.performed, bit2: inputContext.canceled)
        };

        if (!float.IsInfinity(hit.distance))
        {
            data.target = hit.collider.gameObject;
        }

        return data;
    }

    public void OnLeftHand(InputAction.CallbackContext context)
    {
        float3 attackDirection = GetAttackDirection();
        if (context.started)
        {
            if (equipped.EquippableType == EquippableType.weaponMelee)
            {
                if (hitter is AbstractMeleeHitter meleeHitter)
                {
                    meleeHitter.PreBlock(attackDirection);
                }
            }
        }
        else if (context.canceled)
        {
            if (equipped.EquippableType == EquippableType.weaponMelee)
            {
                if (hitter is AbstractMeleeHitter meleeHitter)
                {
                    meleeHitter.PostBlock();
                }
            }
        }
    }

    public void OnSwitchEquipped(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        float delta = context.ReadValue<Vector2>().y;
        equipped.OnUnequip(inventoryController, noritoController, actor);

        GameObject temp;

        if (delta > 0)
        {
            temp = inventoryController.GetNextEquipped(RightHand.transform);
        } 
        else
        {
            temp = inventoryController.GetPrevEquipped(RightHand.transform);
        }
        hitter = temp.GetComponent<IHitter>();
        if (hitter == null || !hitter.IsDirectional)
        {
            directionIndicator.UpdateTarget(BasicHitDirection.None);
        }

        equipped = temp.GetComponent<IEquipMetadata>();
        equipped.OnEquip(inventoryController, noritoController, actor);
    }

    /*  LET'S MANUALLY BUILD THAT FUCKING
        PARENT-SPACE (Player space) -> CHILD-SPACE (Camera space) matrix here
        with EVIL MATH
        
        IMPORTANT:  Must be called again if either transform's scale changes.
                    Rotations and translations are fine.
    */
    void BuildT_pcMatrix()
    {
        // If you know linear algebra, we're playing with transformations and inverses:
        // the format for T_(xy) is a matrix that transfroms from x space to y space
        // T_pc = T_wc * T_wp^-1, given c and p are child and parent
        T_pc = math.mul(camera.transform.worldToLocalMatrix, transform.localToWorldMatrix);
    }

    private float3 CalculateFocalVector(Transform handTransform)
    {
        float focalDistance = maxRayDistance;

        // Find the actual convergence/focal point if it's below maxRayDistance
        // Don't worry, raycasts are cheap
        focalDistance = CalculateRayDistance(focalDistance);

        float3 handPosition = handTransform.transform.localPosition;

        // Evil casting and matrix transformation fuckery
        // handPosition is now magically in camera space
        handPosition = math.transform(T_pc, handPosition);

        /*  Calculate the vector from the origin to the focal point
         *  We're giving a VECTOR so w must be 0
         *  Recall that in a 4x4 transform matrix:
         *      w = 0: (x, y, z, 0), xyz is a vector
         *      w = 1: (x, y, z, 1), xyz is a point in a space
         *
         *  Thank you GLSL indian man from CPSC 314. ありがとう！
         */
        float4 focalVector = new float4(math.forward() * focalDistance - handPosition, 0);

        // Return to world space
        focalVector = math.mul(camera.transform.localToWorldMatrix, focalVector);

        return math.normalize(focalVector).xyz;
    }

    private GameObject GetHand(Hand hand)
    {
        switch (hand)
        {
            case Hand.Left:
                return LeftHand;
            default:
                return RightHand;
        }
        //return Hands[(int)hand];
    }

    private float CalculateRayDistance(float maxDistance)
    {
        float rayDistance = RaycastFromCamera(maxRayDistance).distance;
        return math.min(maxDistance, rayDistance);
    }

    private RaycastHit RaycastFromCamera(uint maxDistance = 500)
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, rayLayer))
        {
            return hit;
        }
        RaycastHit hitData = default;
        hitData.point = MathHelpers.NaN3;
        hitData.normal = MathHelpers.NaN3;
        hitData.distance = math.INFINITY;
        return hitData;
    }

    private float3 GetAttackDirection()
    {
        return cameraController.deltaVelocity;
    }

    private void OnDrawGizmos()
    {
        float3 directionR = CalculateFocalVector(GetHand(Hand.Right).transform);
        float3 directionL = CalculateFocalVector(GetHand(Hand.Left).transform);
        float focalDistance = CalculateRayDistance(maxRayDistance);
        float zDeltaR = RightHand.transform.localPosition.z - camera.transform.localPosition.z;
        float zDeltaL = LeftHand.transform.localPosition.z - camera.transform.localPosition.z;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(RightHand.transform.position,
                        directionR * (focalDistance - zDeltaR));

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(LeftHand.transform.position,
                directionL * (focalDistance - zDeltaL));

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(camera.transform.position, camera.transform.forward * focalDistance);
    }
}
