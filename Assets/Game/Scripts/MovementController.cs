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
        movementSystem.SetMovement(currentOrder.movement);
        movementSystem.SetUseThrottles(currentOrder.leftAdditionalMovement,currentOrder.rightAdditionalMovement);
        movementSystem.ProcessMovement();
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(20, 30, 200, 200), 
            $"max speed = {movementSystem.maxSpeed},\n speed = {GetComponent<Rigidbody2D>().velocity.magnitude}\n" +
            $"calculated acceleration = {movementSystem.CalculatedAcceleration}\n" +
            $"angular velocity = {GetComponent<Rigidbody2D>().angularVelocity}\n" +
            $"Temperature = {GetComponent<Shell>().Temperature}");
    }
}