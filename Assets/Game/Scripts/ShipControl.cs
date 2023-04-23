using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipControl : MonoBehaviour
{
    
    private VirtualPlayer player;
    private ShipOrder currentOrder;
    private CameraOrder currentCameraOrder;

    private Vector2 moveVec;

    private Vector2 mousePos;
    private void Awake()
    {
        player = GetComponent<VirtualPlayer>();
        currentOrder = new ShipOrder();
        currentCameraOrder = new CameraOrder();
    }


    private void Update()
    { 
        currentOrder.aim = Camera.main.ScreenToWorldPoint(mousePos);
        player.UpdateOrder(currentOrder);
        player.UpdateCameraOrder(currentCameraOrder);
    }

    private void FixedUpdate()
    {
        //speed += moveVec * (acceleration * Time.fixedDeltaTime);
        //body.MovePosition(body.position+=moveVec*5*Time.deltaTime);
        
    }

    public void Move(InputAction.CallbackContext callbackContext)
    {
        currentOrder.movement = callbackContext.ReadValue<Vector2>();
        currentCameraOrder.movement = callbackContext.ReadValue<Vector2>();
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
    
    public void MousePosition(InputAction.CallbackContext callbackContext)
    {
        mousePos = callbackContext.ReadValue<Vector2>();
        currentCameraOrder.mouseAim = callbackContext.ReadValue<Vector2>();
    }
    
    public void CameraModeChange(InputAction.CallbackContext callbackContext)
    {
        currentCameraOrder.changeMode = callbackContext.performed;
    }
    
    public void Zoom(InputAction.CallbackContext callbackContext)
    {
        currentCameraOrder.zoom = callbackContext.ReadValue<float>();
    }
}
