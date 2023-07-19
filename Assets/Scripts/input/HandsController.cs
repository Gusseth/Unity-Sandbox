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
        Left,
        Right
    }

    [SerializeField] GameObject LeftHand;
    [SerializeField] GameObject RightHand;
    [SerializeField] GameObject Ball;
    [SerializeField] new Camera camera;
    [SerializeField] uint maxRayDistance;
    [SerializeField] float ballSpeed;

    // TRS Matrix to transfer from Player space to Camera space
    Matrix4x4 T_pc;

    // Start is called before the first frame update
    void Awake()
    {
        BuildT_pcMatrix();
    }

    // Update is called once per frame
    void OnRightHand()
    {
        Vector3 direction = CalculateFocalVector(Hand.Right);
        GameObject ball = Instantiate(Ball);
        ball.transform.position = RightHand.transform.position;
        RayMovement ballMovement = ball.GetComponent<RayMovement>();
        ballMovement.velocity = direction * ballSpeed;
        ballMovement.isMoving = true;
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

    private Vector3 CalculateFocalVector(Hand hand)
    {
        Vector3 fowardVector = Vector3.forward;
        float focalDistance = maxRayDistance;

        // Find the actual convergence/focal point if it's below maxRayDistance
        // Don't worry, raycasts are cheap
        focalDistance = CalculateRayDistance(focalDistance);

        // Vector 3 because there's no math.mul(float3, float) function
        Vector3 handPosition = RightHand.transform.localPosition;


        /*  We're giving a point so w must be 1
            Recall that in a 4x4 transform matrix:
                w = 0: (x, y, z) is a vector
                w = 1: (x, y, z) is a point in a space

            Thank you GLSL indian man from CPSC 314
        */
        float4 p_hand = new float4(handPosition, 1);

        // Evil casting and matrix transformation fuckery
        // handPosition is now magically in camera space
        handPosition = math.mul(T_pc, p_hand).xyz;

        // Calculate the vector from the origin to the focal point
        float4 focalVector = new float4(Vector3.forward * focalDistance - handPosition, 0);

        // Return to world space
        //focalVector = camera.transform.TransformDirection(focalVector);
        focalVector = math.mul(camera.transform.localToWorldMatrix, focalVector);

        return math.normalize(focalVector).xyz;
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
        Vector3 direction = CalculateFocalVector(Hand.Right);
        float focalDistance = CalculateRayDistance(maxRayDistance);
        float zDelta = RightHand.transform.localPosition.z - camera.transform.localPosition.z;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(RightHand.transform.position,
                        direction * (focalDistance - zDelta));

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(camera.transform.position, camera.transform.forward * focalDistance);
    }
}
