using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilControl : ObjectProperty
{
    public RecoilControl(float baseValue) : base("RecoilControl", "RC", 13, "RecoilControl", float.MinValue, float.MinValue, baseValue)
    {
    }
}
