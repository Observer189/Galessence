using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    protected VirtualPlayer owner;
    public ShipInfo info;
    public Transform cameraTarget;

    public PropertyManager PropertyManager { get; private set; }

    private Rigidbody2D body;
    private ShipOrder currentOrder;

    private IShipActionController[] controllers;
    
    public bool IsAlive { get; private set; } = true;

    public VirtualPlayer Owner
    {
        get => owner;
        set => owner = value;
    }
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        controllers = GetComponents<IShipActionController>();
        
    }

    private void Start()
    {
        PropertyManager = GetComponent<PropertyManager>();
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
        //Debug.Log(order.movement);
    }

    public void Kill()
    {
        IsAlive = false;
    }
}

public enum ShipOwnerType
{
    Player, AI
}
