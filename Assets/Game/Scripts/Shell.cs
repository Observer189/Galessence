using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;


public class Shell : PropertyObject
{
    [Tooltip("Теплоемкость объекта")]
    public float thermalCapacity;

    [Tooltip("Стартовая температура тела")]
    public float baseTemperature;

    public Gradient gradient;
    
    private ObjectProperty thermalEnergy;

    private Rigidbody2D body;

    public float Temperature => thermalEnergy.GetCurValue() / (thermalCapacity * body.mass);
    
    public override void Initialize()
    {
        base.Initialize();
        body = GetComponent<Rigidbody2D>();
        thermalEnergy =
            _propertyManager.AddProperty("ThermalEnergy", 
                float.MaxValue, baseTemperature * thermalCapacity * body.mass);
        
    }

    private void Update()
    {
        var cooling = ThermalEnvironment.Instance.Cooling * Temperature;
        var heating = ThermalEnvironment.Instance.Emission;

        heating -= cooling;
        thermalEnergy.ChangeCurValue(heating);
        var sprite = GetComponent<SpriteRenderer>();
        if (Input.GetKey(KeyCode.Space))
        {
            sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
                sprite.color = gradient.Evaluate(Temperature / 2000);
        }
        else
        {
            if (sprite != null)
                sprite.color = Color.white;
        }
    }
}
