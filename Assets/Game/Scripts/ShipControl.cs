using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipControl : MonoBehaviour
{
    
    private ShipController shipController;
    private ShipOrder currentOrder;

    private Vector2 moveVec;
    private void Awake()
    {
        shipController = GetComponent<ShipController>();
        currentOrder = new ShipOrder();
    }


    private void Update()
    {
       shipController.UpdateOrder(currentOrder);
    }

    private void FixedUpdate()
    {
        //speed += moveVec * (acceleration * Time.fixedDeltaTime);
        //body.MovePosition(body.position+=moveVec*5*Time.deltaTime);
        
    }

    public void Move(InputAction.CallbackContext callbackContext)
    {
        currentOrder.movement = callbackContext.ReadValue<Vector2>();
    }
    
    public void MainWeapon(InputAction.CallbackContext callbackContext)
    {
        currentOrder.mainWeapon = callbackContext.performed;
    }
    public void SecondaryWeapon(InputAction.CallbackContext callbackContext)
    {
        currentOrder.secondaryWeapon = callbackContext.performed;
    }
    
    public void LeftThrottle(InputAction.CallbackContext callbackContext)
    {
        currentOrder.leftAdditionalMovement = callbackContext.performed;
    }
    public void RightThrottle(InputAction.CallbackContext callbackContext)
    {
        currentOrder.rightAdditionalMovement = callbackContext.performed;
    }
    
}
