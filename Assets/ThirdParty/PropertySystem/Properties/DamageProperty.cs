using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageProperty : ObjectProperty
{
    public DamageProperty(float baseValue) : base("Damage","DMG", 4, "Damage",float.MinValue,float.MinValue, baseValue)
    {
    }
}
