using System;
using System.Collections.Generic;

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
}
