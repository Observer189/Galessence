using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClassicMovement : MonoBehaviour
{
   

    public Transform forwardDir;
    public float mainEnginePower;
    public float sideThrottlesPower;
    public float maxSpeed;
    public float angularPower;
    public float angularSpeed;

    public float MaxSpeed=>maxSpeed;
    public float CalculatedAcceleration => calculatedAcceleration;
    
    private Vector2 speed;
    //Переменная используемая в случае если нам необходимо довести поворот объекта до определенной градусной меры
    //Например при повороте ракеты
    private float? targetRotation=null;
    private Rigidbody2D shipBody;
    private Vector2 targetMovement;
    private (bool, bool) throttleUse;

    private float lastFrameSpeed;
    private float calculatedAcceleration;

    private Shell shell;
    
    private void Awake()
    {
        shipBody = GetComponent<Rigidbody2D>();
        shell = GetComponent<Shell>();
    }

    void Start()
    {
        lastFrameSpeed = shipBody.velocity.magnitude;
    }

    public void SetMovement(Vector2 movement)
    {
        targetMovement = movement;
    }

    public void SetUseThrottles(bool left, bool right)
    {
        throttleUse = (left, right);
    }
    /// <summary>
    /// Заставляет объект поворачивать четко на заданные координаты
    /// </summary>
    /// <param name="target"></param>
    public void SetRotationTo(Vector2 target)
    {
        var dirToTarget = target - shipBody.position;

        var angle = Vector2.SignedAngle(forwardDir.up, dirToTarget);
        //Debug.Log(angularSpeed*Time.deltaTime*2);
        //Debug.Log(angle);
        if (Mathf.Abs(angle)<angularSpeed*Time.deltaTime*2)
        {
            targetMovement.x = 0;
            
            targetRotation = Vector2.Angle(Vector2.up, dirToTarget);
        }
        else if(angle<0)
        {
            targetMovement.x = 1;
        }
        else
        {
            targetMovement.x = -1;
        }
    }

    public void ProcessMovement()
    {
        calculatedAcceleration = (shipBody.velocity.magnitude - this.lastFrameSpeed)/Time.deltaTime;
        lastFrameSpeed = shipBody.velocity.magnitude;
        if (targetMovement.y > 0)
        {
            //shipBody.AddForce(forwardDir.up*enginePower*Time.deltaTime,ForceMode2D.Impulse);
            shell.AddImpulse(forwardDir.up*mainEnginePower*Time.deltaTime);
        }

        if (throttleUse is { Item1: true, Item2: false })
        {
            shell.AddImpulse(-forwardDir.right*sideThrottlesPower*Time.deltaTime);
        }
        else if(throttleUse is { Item2: true, Item1: false })
        {
            shell.AddImpulse(forwardDir.right*sideThrottlesPower*Time.deltaTime);
        }
        //Debug.Log($"Target: {-targetMovement.x*angularSpeed}, Current: {shipBody.angularVelocity}, Torque: {((-targetMovement.x*angularSpeed)-shipBody.angularVelocity)}");
        var targetAngularSpeed = (-targetMovement.x * angularSpeed);
        if (targetRotation != null) targetAngularSpeed = targetRotation.Value;
        var torque = (targetAngularSpeed - shipBody.angularVelocity) * Mathf.Deg2Rad * shipBody.inertia;
        torque = Mathf.Sign(torque)*Mathf.Min(Mathf.Abs(torque), angularPower);
        shipBody.AddTorque(torque,ForceMode2D.Impulse);
        

        targetRotation = null;
    }
    
}
