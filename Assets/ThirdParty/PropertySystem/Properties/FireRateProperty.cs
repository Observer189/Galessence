using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateProperty : ObjectProperty
{
    public FireRateProperty(float baseValue) : base("FireRate","FR", 6, "FireRate", 0.0001f,0,baseValue)
    {
    }
}
