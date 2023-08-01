using System;
using System.Collections.Generic;
using UnityEngine;

public static class EnumHelpers
{
    public static BasicHitDirection ToBasicHitDirection(Vector3 axis)
    {
        BasicHitDirection attackDirection;
        if (axis.x != 0)
        {
            if (axis.x > 0)
            {
                attackDirection = BasicHitDirection.Right;
            }
            else
            {
                attackDirection = BasicHitDirection.Left;
            }
        }
        else
        {
            if (axis.y > 0)
            {
                attackDirection = BasicHitDirection.Up;
            }
            else
            {
                attackDirection = BasicHitDirection.Down;
            }
        }

        return attackDirection;
    }
}

