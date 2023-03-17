using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyResistance : ObjectProperty
{
    public EnergyResistance(float baseValue) : base("EnergyResistance", "ER", 21, "EnergyResistance", float.MinValue, float.MinValue, baseValue)
    {
    }
}
