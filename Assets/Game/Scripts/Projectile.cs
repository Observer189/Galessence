using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public EffectOnTouch EffectOnTouch { get; private set; }
    public Shell Shell { get; private set; }
    public Health Health { get; private set; }

    private void Awake()
    {
        EffectOnTouch = GetComponent<EffectOnTouch>();
        Shell = GetComponent<Shell>();
        Health = GetComponent<Health>();
    }

  
}
