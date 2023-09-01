using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class MathHelpers
{
    public const float piOverTwo = math.PI / 2;
    public const float twoPi = math.PI * 2;

    public static readonly float2 NaN2 = new float2(math.NAN);
    public static readonly float3 NaN3 = new float3(math.NAN);
    public static readonly float4 NaN4 = new float4(math.NAN);

    public static byte BoolToFlag8(bool bit7 = false, bool bit6 = false, bool bit5 = false, bool bit4 = false, bool bit3 = false, bool bit2 = false, bool bit1 = false, bool bit0 = false)
    {
        byte x = (byte)(bit0 ? 0b1 : 0b0);
        if (bit1)
            x |= 0b0010;
        if (bit2)
            x |= 0b0100;
        if (bit3)
            x |= 0b1000;
        if (bit4)
            x |= 0b00010000;
        if (bit5)
            x |= 0b00100000;
        if (bit6)
            x |= 0b01000000;
        if (bit7)
            x |= 0b10000000;
        return x;
    }

    public static byte ChangeFlag4(byte flag, int index, bool value) 
    {
        byte x = (byte)(value ? 0b1 : 0b0);
        return (byte)(flag & (x << index));
    }

    public static Int16 ChangeFlag16(Int16 flag, int index, bool value)
    {
        byte x = (byte)(value ? 0b1 : 0b0);
        return (Int16)(flag & (x << index));
    }

    public static Int32 ChangeFlag32(Int32 flag, int index, bool value)
    {
        byte x = (byte)(value ? 0b1 : 0b0);
        return (Int32)(flag & (x << index));
    }

    public static Int64 ChangeFlag64(Int64 flag, int index, bool value)
    {
        byte x = (byte)(value ? 0b1 : 0b0);
        return (Int64)(flag & (x << index));
    }

    public static bool FlagContains(byte inputFlags, byte filter)
    {
        return (inputFlags & filter) != 0;
    }

    public static bool FlagContains(Int16 inputFlags, Int16 filter)
    {
        return (inputFlags & filter) != 0;
    }

    public static bool FlagContains(Int32 inputFlags, Int32 filter)
    {
        return (inputFlags & filter) != 0;
    }

    public static bool FlagContains(Int64 inputFlags, Int64 filter)
    {
        return (inputFlags & filter) != 0;
    }

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
