using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class that describes input provided to ship controller
 * Universal for player input and AI
 */
public class ShipOrder
{
    //Describes ship movement
    public Vector4 movement;
    //Determine how movement system considers input movement vector
    //If RotationTo then movement vector is considered as point in world space that ship want to be headed
    //as follows: y is gas/brake and (x,z) is (x,y) coordinates of rotation target in world space
    //If direct then considered y-axis as gas/brake and x as rotate left/right
    //If TargetSpeed then y - axis is target speed for direction that ship is headed
    //x - target rotation speed
    //z - axis is target speed for direction perpendicular for one that ship is headed
    //If TargetSpeedWithRotationTo then (x,w) - point in 2d space that ship should rotate to
    //y, z as in normal TargetSpeedMode
    public MovementOrderType movementOrderType;
    //Mouse pointer in world space or target for ship's weapons for AI
    public Vector2 aim;
    //Whether main weapon is firing or not
    public bool mainWeapon;
    //Whether secondary weapon is firing or not
    public bool secondaryWeapon;
    //Whether additional left movement is on('q' button on keyboard)
    public bool leftAdditionalMovement;
    //Whether additional right movement is on('q' button on keyboard)
    public bool rightAdditionalMovement;
    //whether special button 1 is pressed(shift on keyboard)
    public bool special1;
    //whether special button 2 is pressed(space on keyboard)
    public bool special2;

    public ShipOrder()
    {
    }

    public ShipOrder GetCopy()
    {
        return new ShipOrder(movement, movementOrderType, aim, mainWeapon, secondaryWeapon, leftAdditionalMovement,
            rightAdditionalMovement, special1, special2);
    }

    public ShipOrder(Vector4 movement, MovementOrderType movementOrderType, Vector2 aim, bool mainWeapon, 
        bool secondaryWeapon, bool leftAdditionalMovement, bool rightAdditionalMovement, bool special1, bool special2)
    {
        this.movement = movement;
        this.movementOrderType = movementOrderType;
        this.aim = aim;
        this.mainWeapon = mainWeapon;
        this.secondaryWeapon = secondaryWeapon;
        this.leftAdditionalMovement = leftAdditionalMovement;
        this.rightAdditionalMovement = rightAdditionalMovement;
        this.special1 = special1;
        this.special2 = special2;
    }
}

public enum MovementOrderType
{
    Direct, RotationTo, TargetSpeed, TargetSpeedWithRotationTo
}
