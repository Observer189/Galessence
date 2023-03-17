using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPProperty : ObjectProperty
{
    public HPProperty(float baseValue) : base("Hitpoints","HP", 1, "Hp",1,0, baseValue)
    {
        
    }
}
