using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : PropertyObject
{
    [Tooltip("Время необходимое реактору чтобы начать работу после последней траты энергии")]
    public float recoverDelay;

    protected float lastEnergySpendTime;

    private ObjectProperty energy;
    protected override void Start()
    {
        base.Start();
        energy = _propertyManager.GetPropertyById(8);
        energy.RegisterChangeCallback(OnEnergyChanged);
    }

    private void Update()
    {
        if (Time.time - lastEnergySpendTime > recoverDelay)
        {
            var eGen = _propertyManager.GetPropertyById(9);
            energy.ChangeCurValue(eGen.GetCurValue() * Time.deltaTime);
        }

        //Debug.Log(energy.GetCurValue());
    }

    protected void OnEnergyChanged(float oldCurValue, float newCurValue, float oldValue, float newValue)
    {
        if (oldCurValue > newCurValue)
        {
            lastEnergySpendTime = Time.time;
        }
    }
}
