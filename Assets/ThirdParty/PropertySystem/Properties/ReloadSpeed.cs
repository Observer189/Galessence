using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSpeed : ObjectProperty
{
    public ReloadSpeed(float baseValue) : base("ReloadSpeed", "RS", 10, "ReloadSpeed", 0, 0, baseValue)
    {
    }
}
