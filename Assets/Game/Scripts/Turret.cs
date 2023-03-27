using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour, IShipActionController
{
    public Transform turretBaseTransform;
    public Transform turretTransform;

    public bool isRotationLimited;
    public float leftRotationLimit;
    public float rightRotationLimit;
    public float turretRotationSpeed;
    
    private ShipOrder currentOrder;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentOrder==null) return;
        
        var angleToEnemy = FunctionPack.AngleToPoint(turretBaseTransform.position, currentOrder.aim);

        angleToEnemy = FunctionPack.NormalizeAngle(angleToEnemy);
        
        var incAngle =
            FunctionPack.FindRotationDirection
            (turretTransform.rotation.eulerAngles.z,
                angleToEnemy,
                turretRotationSpeed * Time.deltaTime) * turretRotationSpeed * Time.deltaTime;

        //turretTransform.Rotate(0, 0, incAngle);

        if (isRotationLimited)
        {
            if (incAngle > 0)
            {
                if (FunctionPack.FindRotationDirection(turretTransform.eulerAngles.z + incAngle,
                turretBaseTransform.eulerAngles.z+leftRotationLimit,0.0001f) != Mathf.Sign(incAngle))
                {
                    incAngle = 0;
                }
            }
            else if (incAngle < 0)
            {
                if (FunctionPack.FindRotationDirection(turretTransform.eulerAngles.z + incAngle,
                        turretBaseTransform.eulerAngles.z-rightRotationLimit,0.0001f) != Mathf.Sign(incAngle))
                {
                    incAngle = 0;
                }
            }
            
            
        } 
        turretTransform.Rotate(0, 0, incAngle);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(turretTransform.position, turretTransform.position+turretTransform.up*10);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(turretBaseTransform.position, (Vector2)turretBaseTransform.position+((Vector2)turretBaseTransform.up).MMRotate(leftRotationLimit)*10);
        Gizmos.DrawLine(turretBaseTransform.position, (Vector2)turretBaseTransform.position+((Vector2)turretBaseTransform.up).MMRotate(-rightRotationLimit)*10);
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

    public ShipActionControllerType ControllerType { get; }
}
