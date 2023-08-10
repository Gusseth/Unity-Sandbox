using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class MathHelpers
{
    public static readonly float piOverTwo = math.PI / 2;
    public static readonly float twoPi = math.PI * 2;

    public static readonly float2 NaN2 = new float2(math.NAN);
    public static readonly float3 NaN3 = new float3(math.NAN);
    public static readonly float4 NaN4 = new float4(math.NAN);

    public static Enum FloatDirection(float x, Enum positiveDirection, Enum negativeDirection)
    {
        if (x < 0)
        {
            return negativeDirection;
        }
        else
        {
            return positiveDirection;
        }
    }

    public static float3 CardinalizeDirection(float3 direction)
    {
        // Using vector magic!
        float3 ne_axis = math.normalize(new float3(1, 1, 0));  // Vector towards NE direction
        float3 nw_axis = math.normalize(new float3(-1, 1, 0)); // Vector towards NW direction

        // Recall that the dot product returns positive if the 'direction' vector 
        // is going towards the same general direction as the 'nx_axis' 
        // Dot products return negative if the vector 'direction' points away from the axis.
        float ne = math.dot(ne_axis, direction);
        float nw = math.dot(nw_axis, direction);

        // Refer to CardinalizeDirection Guide.png

        if (ne < 0)
        {
            if (nw < 0)
            {
                // Negative dot(ne), dot(nw)
                return Vector3.down;
            }
            else
            {
                // Negative dot(ne), positive dot(nw)
                return Vector3.left ;
            }
        }
        else
        {
            if (nw < 0)
            {
                // Positive dot(ne), negative dot(nw)
                return Vector3.right;
            }
            else
            {
                // Positive dot(ne), dot(nw)
                return Vector3.up;
            }
        }
    }
}
