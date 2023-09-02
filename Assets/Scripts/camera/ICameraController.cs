using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

public interface ICameraController
{
    public float3 DeltaVelocity { get; }
}
