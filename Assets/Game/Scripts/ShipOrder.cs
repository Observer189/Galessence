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
    public Vector3 movement;
    //If true then movement vector is considered as point in world space that ship want to be headed
    //as follows: y is gas/brake and (x,z) is (x,y) coordinates of rotation target in world space
    //If false than considered y-axis as gas/brake and x as rotate left/right
    public bool movementHasRotationDirection;
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
        return new ShipOrder(movement, movementHasRotationDirection, aim, mainWeapon, secondaryWeapon, leftAdditionalMovement,
            rightAdditionalMovement, special1, special2);
    }

    public ShipOrder(Vector3 movement, bool movementHasRotationDirection, Vector2 aim, bool mainWeapon, 
        bool secondaryWeapon, bool leftAdditionalMovement, bool rightAdditionalMovement, bool special1, bool special2)
    {
        this.movement = movement;
        this.movementHasRotationDirection = movementHasRotationDirection;
        this.aim = aim;
        this.mainWeapon = mainWeapon;
        this.secondaryWeapon = secondaryWeapon;
        this.leftAdditionalMovement = leftAdditionalMovement;
        this.rightAdditionalMovement = rightAdditionalMovement;
        this.special1 = special1;
        this.special2 = special2;
    }
}
