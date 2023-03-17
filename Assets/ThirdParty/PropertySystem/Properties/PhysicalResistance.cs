using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalResistance : ObjectProperty
{
    public PhysicalResistance(float baseValue) : base("PhysicalResistance", "PR", 20, "PhysicalResistance", float.MinValue, float.MinValue, baseValue)
    {
    }
}
