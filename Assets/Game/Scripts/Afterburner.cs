using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

public class Afterburner : PropertyObject, IShipActionController
{
    public EffectDescription[] afterburnEffects;

    protected bool isActivated = false;

    protected ShipOrder shipOrder;
    
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (shipOrder != null)
        {
            if (shipOrder.secondaryWeapon)
            {
                if (!isActivated)
                {
                    foreach (var effect in afterburnEffects)
                    {
                        _propertyManager.AddEffect(new Effect(effect,_propertyManager));
                    }

                    isActivated = true;
                }
            }
            else
            {
                if (isActivated)
                {
                    foreach (var effect in afterburnEffects)
                    {
                        _propertyManager.RemoveEffect(effect.Id);
                    }

                    isActivated = false;
                }
            }
        }
    }

    public void UpdateOrder(ShipOrder order)
    {
        shipOrder = order;
    }

    public ShipActionControllerType ControllerType { get; }
}
