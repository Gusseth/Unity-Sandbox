using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class CastableHelpers
{
    public static bool CheckFlag(CastingData castData, params InputFlags[] flags)
    {
        byte filter = 0;
        for (int i = 0; i < flags.Length; i++)
        {
            filter |= (byte)flags[i];
        }

        return MathHelpers.FlagContains(castData.inputFlags, filter);
    }
}
