using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeProperty : ObjectProperty
{
    public RangeProperty(float baseValue) : base("Range", "RNG", 8, "Range", 0.5f, 1, baseValue)
    {
    }
}
