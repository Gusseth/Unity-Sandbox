using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class HandsController : MonoBehaviour
{
    enum Hand
    {
        Left = 0,
        Right = 1
    }

    [SerializeField] GameObject LeftHand;
    [SerializeField] GameObject RightHand;
    //[SerializeField] List<GameObject> Hands;
    [SerializeField] GameObject Ball;
    [SerializeField] new Camera camera;
    [SerializeField] uint maxRayDistance;
    [SerializeField] float ballSpeed;

    // TRS Matrix to transfer from Player space to Camera space
    float4x4 T_pc;
    IHitter hitter;

    // Start is called before the first frame update
    void Awake()
    {
        BuildT_pcMatrix();
    }

    // Update is called once per frame
    void OnRightHand()
    {
        float3 direction = CalculateFocalVector(Hand.Right);
        GameObject ball = Instantiate(Ball);
        ball.transform.position = RightHand.transform.position;
        RayMovement ballMovement = ball.GetComponent<RayMovement>();
        ballMovement.velocity = direction * ballSpeed;
        ballMovement.isMoving = true;
    }

    void OnLeftHand()
    {
        // float3 direction = CalculateFocalVector(Hand.Left);
        //Debug.Log("Left Hand not yet implemented!!");
        hitter ??= GetHand(Hand.Left).GetComponentInChildren<IHitter>();
        if (hitter.Attacking)
            hitter.PostAttack();
        hitter.Attacking = !hitter.Attacking;
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
