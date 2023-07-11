using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Utils;
using MoreMountains.Tools;
using UnityEngine;

public class ClassicMovement : PropertyObject, IMovementSystem
{
   

    public Transform forwardDir;
    [Tooltip("Соотношение мощности двигателей при движении назад к мощности при движении вперед")]
    public float backEngineModifier;
    //public float mainEnginePower;
    public float sideThrottlesPower;
    [Tooltip("Угол в пределах которого считается, что мы точно развернулись на цель")]
    public float rotationEpsilon;
    /*public float maxSpeed;
    public float angularPower;
    public float angularSpeed;*/

    public ParticleSystem[] forwardEngines;
    public ParticleSystem[] backEngines;
    public ParticleSystem[] rightEngines;
    public ParticleSystem[] leftEngines;
    public ParticleSystem[] turnRightEngines;
    public ParticleSystem[] turnLeftEngines;

    protected HashSet<ParticleSystem> activatedEngines;
    protected HashSet<ParticleSystem> deactivatedEngines;

    public float MainEnginePower=>_propertyManager.GetPropertyById(4).GetCurValue();
    public float MaxSpeed=>_propertyManager.GetPropertyById(5).GetCurValue();
    public float AngularPower => _propertyManager.GetPropertyById(6).GetCurValue();
    public float AngularSpeed => _propertyManager.GetPropertyById(7).GetCurValue();
    public float CalculatedAcceleration => calculatedAcceleration;
    
    private Vector2 speed;
    //Переменная используемая в случае если нам необходимо довести поворот объекта до определенной градусной меры
    //Например при повороте ракеты
    private float? targetRotation=null;
    private bool isInTargetSpeedMode;
    private Rigidbody2D shipBody;
    private Vector4 targetMovement;
    private (bool, bool) throttleUse;

    private float lastFrameSpeed;
    private float lastFrameAngularSpeed;
    private float calculatedAcceleration;

    private Shell shell;

    private void Awake()
    {
        base.Awake();
        shipBody = GetComponent<Rigidbody2D>();
        shell = GetComponent<Shell>();
        activatedEngines = new HashSet<ParticleSystem>();
        deactivatedEngines = new HashSet<ParticleSystem>();
    }

    void Start()
    {
        base.Start();
        lastFrameSpeed = shipBody.velocity.magnitude;
    }

    public void SetMovement(Vector4 movement)
    {
        targetMovement = movement;
    }

    public void SetTargetSpeedMode(bool flag)
    {
        isInTargetSpeedMode = flag;
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
        var angularSpeed = _propertyManager.GetPropertyById(7).GetCurValue();
        
        var dirToTarget = target - shipBody.position;

        var angle = Vector2.SignedAngle(forwardDir.up, dirToTarget);
        //Debug.Log(angularSpeed*Time.deltaTime*2);
        //Debug.Log(angle);
        if (Mathf.Abs(angle)<angularSpeed*Time.deltaTime*6)
        {
            targetMovement.x = 0;
            
            //targetRotation = Vector2.Angle(Vector2.up, dirToTarget);
            targetRotation = angle;
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

    public void CalculateAngularDesireSpeedByRotationTo()
    {
        ///Рассчитываем максимальную скорость которую мы можем развить или наоборот сбросить
        var maxMomentaryAngularSpeed = AngularPower / (Mathf.Deg2Rad * shipBody.inertia);

        var targetAngularSpeed = (-targetMovement.x * AngularSpeed);
        if (targetRotation != null)
        {
            ///Если мы и так достаточно точно развенуты на цель, то сбрасываем скорость до нуля
            if (Mathf.Abs(targetRotation.Value) < rotationEpsilon)
            {
                targetRotation = 0;
                targetAngularSpeed = 0;
            }
            else
            {
                targetAngularSpeed = Mathf.Sign(targetRotation.Value) * maxMomentaryAngularSpeed;
            }
            
        }

        targetMovement.x = targetAngularSpeed;
    }

    public void ProcessMovement()
    {
        if (!isInTargetSpeedMode)
        {
            calculatedAcceleration = (shipBody.velocity.magnitude - this.lastFrameSpeed) / Time.deltaTime;
            lastFrameSpeed = shipBody.velocity.magnitude;
            if (targetMovement.y > 0)
            {
                //shipBody.AddForce(forwardDir.up*enginePower*Time.deltaTime,ForceMode2D.Impulse);
                shell.AddImpulse(forwardDir.up * MainEnginePower * Time.deltaTime);
            }
            else if (targetMovement.y < 0)
            {
                shell.AddImpulse(-forwardDir.up * backEngineModifier * MainEnginePower * Time.deltaTime);
            }

            if (throttleUse is { Item1: true, Item2: false })
            {
                shell.AddImpulse(-forwardDir.right * sideThrottlesPower * Time.deltaTime);
            }
            else if (throttleUse is { Item2: true, Item1: false })
            {
                shell.AddImpulse(forwardDir.right * sideThrottlesPower * Time.deltaTime);
            }

            //Debug.Log($"Target: {-targetMovement.x*angularSpeed}, Current: {shipBody.angularVelocity}, Torque: {((-targetMovement.x*angularSpeed)-shipBody.angularVelocity)}");
            ///Рассчитываем максимальную скорость которую мы можем развить или наоборот сбросить
            var maxMomentaryAngularSpeed = AngularPower / (Mathf.Deg2Rad * shipBody.inertia);

            var targetAngularSpeed = (-targetMovement.x * AngularSpeed);
            if (targetRotation != null)
            {
                ///Если мы и так достаточно точно развенуты на цель, то сбрасываем скорость до нуля
                if (Mathf.Abs(targetRotation.Value) < rotationEpsilon)
                {
                    targetRotation = 0;
                    targetAngularSpeed = 0;
                }
                else
                {
                    targetAngularSpeed = Mathf.Sign(targetRotation.Value) * maxMomentaryAngularSpeed;
                }

                //Debug.Log(targetRotation.Value);
                //Debug.Log((AngularPower/shipBody.inertia)*Mathf.Rad2Deg);
                /*Debug.Log($"Target angular speed: {shipBody.angularVelocity}");
                Debug.Log($"Estimated acceleration: {targetAngularSpeed - shipBody.angularVelocity}");
                Debug.Log($"Calculated acceleration: {shipBody.angularVelocity - lastFrameAngularSpeed}");*/
            }

            lastFrameAngularSpeed = shipBody.angularVelocity;
            //Debug.Log(targetRotation);
            var torque = (targetAngularSpeed - shipBody.angularVelocity) * Mathf.Deg2Rad * shipBody.inertia;
            torque = Mathf.Sign(torque) * Mathf.Min(Mathf.Abs(torque), AngularPower);
            shipBody.AddTorque(torque, ForceMode2D.Impulse);
            //MMDebug.DebugDrawArrow(shipBody.position,shipBody.transform.up,Color.green,10f,1f);
        }
        else
        {
            HandleMovementWithDesireSpeed();
        }


        HandleEngineParticles();
        
        targetRotation = null;
    }

    protected void HandleMovementWithDesireSpeed()
    {
        //Считается, что на момент выполнения процедуры в переменной targetMovement лежит следующий вектор
        //(Желательная скорость поворота(со знаком), желательная скорость корабля сонаправленная с текущим углом поворотаб
        //Желательная скорость корабля перпендикулярная текущему углу поворота)
        
        MMDebug.DebugDrawArrow(transform.position,forwardDir.up*targetMovement.y+forwardDir.right*targetMovement.z,Color.yellow);
            var targetNormalized = 
                new Vector2(targetMovement.y, targetMovement.z).normalized * 
                Mathf.Min(targetMovement.y+targetMovement.z,MaxSpeed);
            targetMovement.SetY(targetNormalized.x);
            targetMovement.SetZ(targetNormalized.y);
            
            MMDebug.DebugDrawArrow(transform.position,forwardDir.up*targetMovement.y+forwardDir.right*targetMovement.z,Color.green);
            var velMag = shipBody.velocity.magnitude;
            var verticalSpeedProjection = (velMag == 0)?0:
                shipBody.velocity.ProjectionTo(forwardDir.up);
            var horizontalSpeedProjection = (velMag == 0)?0:
                shipBody.velocity.ProjectionTo(forwardDir.right);
            
//            Debug.Log("Result: "+targetMovement);
            //Debug.Log("Speed:" + shipBody.velocity);
            if (verticalSpeedProjection < targetMovement.y)
            {
                shell.AddImpulse(forwardDir.up * MainEnginePower * Time.deltaTime);
                ///Эта переменная в контексте движения нам больше не нужна, но мы ее меняем так, чтобы
                /// работала корректная визуализация двигателей
                targetMovement.y = 1;
            }
            else if (verticalSpeedProjection > targetMovement.y)
            {
                shell.AddImpulse(-forwardDir.up * backEngineModifier * MainEnginePower * Time.deltaTime);
                targetMovement.y = -1;
            }

            if (horizontalSpeedProjection < targetMovement.z)
            {
                shell.AddImpulse(forwardDir.right * sideThrottlesPower * Time.deltaTime);
                throttleUse = (false,true);
            }
            else if (horizontalSpeedProjection > targetMovement.z)
            {
                shell.AddImpulse(-forwardDir.right * sideThrottlesPower * Time.deltaTime);
                throttleUse = (true,false);
            }

            //Debug.Log(targetMovement.y);
            targetMovement.x = Mathf.Sign(targetMovement.x) * Mathf.Min(AngularSpeed, Mathf.Abs(targetMovement.x));
            var torque = (targetMovement.x - shipBody.angularVelocity) * Mathf.Deg2Rad * shipBody.inertia;
            torque = Mathf.Sign(torque) * Mathf.Min(Mathf.Abs(torque), AngularPower);
            shipBody.AddTorque(torque, ForceMode2D.Impulse);
            if (torque<0)
            {
                targetMovement.x = 1;
            }
            else if (torque>0)
            {
                targetMovement.x = -1;
            }
            
            MMDebug.DebugDrawArrow(transform.position,shipBody.velocity,Color.red);
           MMDebug.DebugDrawArrow(transform.position,transform.up*verticalSpeedProjection,Color.magenta);
           MMDebug.DebugDrawArrow(transform.position,transform.right*horizontalSpeedProjection,Color.magenta);
    }

    protected void HandleEngineParticles()
    {
        activatedEngines.Clear();
        
        SwitchEngineParticles(forwardEngines, targetMovement.y > 0);

        SwitchEngineParticles(backEngines, targetMovement.y < 0);

        SwitchEngineParticles(turnRightEngines, targetMovement.x>0 || targetRotation is > 0);
        
        SwitchEngineParticles(turnLeftEngines, targetMovement.x<0 || targetRotation is < 0);
        
        SwitchEngineParticles(rightEngines, throttleUse.Item1);

        SwitchEngineParticles(leftEngines, throttleUse.Item2);

        foreach (var engine in activatedEngines)
        {
            if (!engine.isPlaying)
            {
                engine.Play();
            }
        }
        
        foreach (var engine in deactivatedEngines)
        {
            if (!activatedEngines.Contains(engine) && !engine.isStopped)
            {
                engine.Stop();
            }
        }
    }

    protected void SwitchEngineParticles(ParticleSystem[] engines, bool isOn)
    {
        if (isOn)
        {
            foreach (var engine in engines)
            {
                activatedEngines.Add(engine);
            }
        }
        else
        {
            foreach (var engine in engines)
            {
                deactivatedEngines.Add(engine);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(shipBody!=null)
        Gizmos.DrawLine(shipBody.position,shipBody.transform.position+shipBody.transform.up*100);
    }
}
