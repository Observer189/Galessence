using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeedProperty : ObjectProperty
{
    public MovementSpeedProperty(float baseValue) : base("MovementSpeed", "MS", 14, "MovementSpeed", 0f, 0f, baseValue)
    {
    }
}
