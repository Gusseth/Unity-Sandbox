using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    AbstractActorBase actor;

    // TRS Matrix to transfer from Player space to Camera space
    float4x4 T_pc;
    IHitter hitter;

    // Start is called before the first frame update
    void Awake()
    {
        BuildT_pcMatrix();
    }

    void Start()
    {
        cameraController ??= GetComponent<BetterCameraController>();
        inventoryController ??= GetComponent<IInventoryController>();
        actor ??= GetComponent<AbstractActorBase>();

        GameObject temp = inventoryController.GetEquipped(Hand.Right, RightHand.transform);
        temp.transform.parent = RightHand.transform;
        hitter = temp.GetComponent<IHitter>();
        equipped = temp.GetComponent<IEquipMetadata>();
        equipped.OnEquip(inventoryController, actor);
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
        if (!context.performed) return; // ignore other actions for now, temporarily one shot 

        if (equipped.EquippableType == EquippableType.weaponMagic)
        {
            float3 direction = CalculateFocalVector(Hand.Right);
            GameObject ball = Instantiate(Ball);
            ball.transform.position = RightHand.transform.position;
            RayMovement ballMovement = ball.GetComponent<RayMovement>();
            ballMovement.velocity = direction * ballSpeed;
            ballMovement.isMoving = true;
        } 
        else
        {
            // float3 direction = CalculateFocalVector(Hand.Left);
            //hitter ??= GetHand(Hand.Right).GetComponentInChildren<IHitter>();
            float3 attackDirection = GetAttackDirection();

            if (hitter.Attacking)
                hitter.PostAttack();    // temporary branch, will do it automatically in the future
            else
            {
                hitter.PreAttack(attackDirection);
            }
        }
    }

    public void OnLeftHand(InputAction.CallbackContext context)
    {

    }

    public void OnSwitchEquipped(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        float delta = context.ReadValue<Vector2>().y;
        equipped.OnUnequip(inventoryController, actor);

        GameObject temp;

        if (delta > 0)
        {
            temp = inventoryController.GetNextEquipped(Hand.Right, RightHand.transform);
        } 
        else
        {
            temp = inventoryController.GetPrevEquipped(Hand.Right, RightHand.transform);
        }
        hitter = temp.GetComponent<IHitter>();
        if (hitter == null || !hitter.IsDirectional)
        {
            directionIndicator.UpdateTarget(BasicHitDirection.None);
        }

        equipped = temp.GetComponent<IEquipMetadata>();
        equipped.OnEquip(inventoryController, actor);
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

    private float3 CalculateFocalVector(Hand hand)
    {
        float focalDistance = maxRayDistance;

        // Find the actual convergence/focal point if it's below maxRayDistance
        // Don't worry, raycasts are cheap
        focalDistance = CalculateRayDistance(focalDistance);

        float3 handPosition = GetHand(hand).transform.localPosition;

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
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            maxDistance = hit.distance;
        }

        return maxDistance;
    }

    private float3 GetAttackDirection()
    {
        return cameraController.deltaVelocity;
    }

    private void OnDrawGizmos()
    {
        float3 directionR = CalculateFocalVector(Hand.Right);
        float3 directionL = CalculateFocalVector(Hand.Left);
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
