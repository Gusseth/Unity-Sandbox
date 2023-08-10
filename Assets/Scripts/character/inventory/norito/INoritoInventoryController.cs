using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface INoritoInventoryController
{
    public INoritoInventory NoritoInventory { get; }
    public bool OnCast(CastingData castData);
}