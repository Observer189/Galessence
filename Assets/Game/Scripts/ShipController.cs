using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    private Rigidbody2D body;
    private ShipOrder currentOrder;

    private IShipActionController[] controllers;
    
    public bool IsAlive { get; private set; } = true;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        controllers = GetComponents<IShipActionController>();
    }

    private void Update()
    {
        
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
        foreach (var controller in controllers)
        {
            controller.UpdateOrder(order);
        }
    }

    public void Kill()
    {
        IsAlive = false;
    }
}
