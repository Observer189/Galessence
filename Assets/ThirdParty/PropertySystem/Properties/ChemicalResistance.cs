using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChemicalResistance : ObjectProperty
{
    public ChemicalResistance(float baseValue) : base("ChemicalResistance", "CR", 23, "ChemicalResistance", float.MinValue,float.MinValue, baseValue)
    {
    }
}
