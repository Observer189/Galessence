using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    public ShipOwnerType ControlledBy;
    public Transform cameraTarget; 
    
    private Rigidbody2D body;
    private ShipOrder currentOrder;

    private IShipActionController[] controllers;
    
    public bool IsAlive { get; private set; } = true;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        controllers = GetComponents<IShipActionController>();

        if (ControlledBy == ShipOwnerType.AI)
        {
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<ShipControl>().enabled = false;
            
            var m = GetComponentInChildren<AIMind>();
            m.SetShip(this);
        }
        else if(ControlledBy == ShipOwnerType.Player)
        {
            CameraManager.Instance.SetFollowTarget(cameraTarget);
            var m = GetComponentInChildren<AIMind>();
            Destroy(m.gameObject);
        }
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

public enum ShipOwnerType
{
    Player, AI
}
