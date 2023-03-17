using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyProperty : ObjectProperty
{
    public AccuracyProperty(float baseValue) : base("Accuracy", "A", 12, "Accuracy", 0, 0, baseValue)
    {
    }
}
