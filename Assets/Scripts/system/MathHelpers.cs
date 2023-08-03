using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class MathHelpers
{
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
