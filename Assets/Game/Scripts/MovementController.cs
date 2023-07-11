using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class MovementController : MonoBehaviour, IShipActionController
{
    public ShipActionControllerType ControllerType => ShipActionControllerType.MovementController;
    private ShipOrder currentOrder;

    private ClassicMovement movementSystem;
    private void Awake()
    {
        currentOrder = new ShipOrder();
        movementSystem = GetComponent<ClassicMovement>();
    }

   
    // Update is called once per frame
    void Update()
    {
        if (currentOrder != null)
        {
            if (currentOrder.movementOrderType == MovementOrderType.RotationTo)
            {
                movementSystem.SetMovement(new Vector2(0,currentOrder.movement.y));
                movementSystem.SetRotationTo(new Vector2(currentOrder.movement.x,currentOrder.movement.z));
            }
            else if(currentOrder.movementOrderType == MovementOrderType.Direct)
            {
                movementSystem.SetMovement(currentOrder.movement);
            }
            else if (currentOrder.movementOrderType == MovementOrderType.TargetSpeed)
            {
                movementSystem.SetMovement(currentOrder.movement);
                movementSystem.SetTargetSpeedMode(true);
            }
            else if(currentOrder.movementOrderType == MovementOrderType.TargetSpeedWithRotationTo)
            {
                movementSystem.SetMovement(currentOrder.movement);
                movementSystem.SetRotationTo(new Vector2(currentOrder.movement.x,currentOrder.movement.w));
                movementSystem.CalculateAngularDesireSpeedByRotationTo();
                movementSystem.SetTargetSpeedMode(true);
            }

            movementSystem.SetUseThrottles(currentOrder.leftAdditionalMovement, currentOrder.rightAdditionalMovement);
        }

        movementSystem.ProcessMovement();
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(20, 30, 200, 200), 
            $"max speed = {movementSystem.MaxSpeed},\n speed = {GetComponent<Rigidbody2D>().velocity.magnitude}\n" +
            $"calculated acceleration = {movementSystem.CalculatedAcceleration}\n" +
            $"angular velocity = {GetComponent<Rigidbody2D>().angularVelocity}\n" +
            $"Temperature = {GetComponent<Shell>().Temperature}");
    }
}
