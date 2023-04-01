using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class Afterburner : PropertyObject, IShipActionController
{
    /// <summary>
    /// Эффекты форсажа, если мы используем оба ускорителя
    /// </summary>
    public EffectDescription[] fullAfterburnEffects;
    /// <summary>
    /// Эффекты форсажа, если мы используем один ускоритель
    /// </summary>
    public EffectDescription[] sideAfterburnEffects;
    /// <summary>
    /// Количество тепловой энергии придаваемой кораблю в секунду при форсаже одного ускорителя
    /// </summary>
    public float burnEnergy;

    protected AfterburnState state = AfterburnState.Off;

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
            if (shipOrder.secondaryWeapon || (shipOrder.leftAdditionalMovement && shipOrder.rightAdditionalMovement))
            {
                if (state !=AfterburnState.Full)
                {
                    if (state == AfterburnState.Side)
                    {
                        foreach (var effect in sideAfterburnEffects)
                        {
                            _propertyManager.RemoveEffect(effect.Id);
                        }
                    }

                    foreach (var effect in fullAfterburnEffects)
                    {
                        _propertyManager.AddEffect(new Effect(effect,_propertyManager));
                    }

                    state = AfterburnState.Full;
                }
            }
            else if (shipOrder.leftAdditionalMovement || shipOrder.rightAdditionalMovement)
            {
                if (state != AfterburnState.Side)
                {
                    if (state == AfterburnState.Full)
                    {
                        foreach (var effect in fullAfterburnEffects)
                        {
                            _propertyManager.RemoveEffect(effect.Id);
                        }
                    }
                    
                    foreach (var effect in sideAfterburnEffects)
                    {
                        _propertyManager.AddEffect(new Effect(effect,_propertyManager));
                    }

                    state = AfterburnState.Side;
                    
                }
            }
            else
            {
                if (state != AfterburnState.Off)
                {
                    if (state == AfterburnState.Full)
                    {
                        foreach (var effect in fullAfterburnEffects)
                        {
                            _propertyManager.RemoveEffect(effect.Id);
                        }
                    }
                    if (state == AfterburnState.Side)
                    {
                        foreach (var effect in sideAfterburnEffects)
                        {
                            _propertyManager.RemoveEffect(effect.Id);
                        }
                    }

                    state = AfterburnState.Off;
                }
            }
        }

        switch (state)
        {
            case AfterburnState.Off:
                break;
            case AfterburnState.Side:
                _propertyManager.GetPropertyById(10).ChangeCurValue(burnEnergy*Time.deltaTime);
                break;
            case AfterburnState.Full:
                _propertyManager.GetPropertyById(10).ChangeCurValue(burnEnergy*Time.deltaTime*2);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UpdateOrder(ShipOrder order)
    {
        shipOrder = order;
        if (order != null)
        {
            //Если мы используем форсаж, то переопределяем приказ так, чтобы там было указано, что корабль движется вперед
            if (order.secondaryWeapon || (order.leftAdditionalMovement && order.rightAdditionalMovement))
            {
                order.movement = order.movement.MMSetY(1);
            }
            else if (shipOrder.leftAdditionalMovement)
            {
                order.movement = order.movement.MMSetX(-1);
            }
            else if (shipOrder.rightAdditionalMovement)
            {
                order.movement = order.movement.MMSetX(1);
            }
        }
    }

    public ShipActionControllerType ControllerType { get; }
    
    protected enum AfterburnState
    {
        Off, Side, Full
    }
}


