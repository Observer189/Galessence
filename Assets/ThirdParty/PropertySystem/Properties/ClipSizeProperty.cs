using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipSizeProperty : ObjectProperty
{
    public ClipSizeProperty(float baseValue) : base("Clip size", "CS", 9, "ClipSize", 1, 1, baseValue)
    {
    }
}
