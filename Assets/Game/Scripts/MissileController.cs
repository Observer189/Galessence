using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    public bool startActive;
    public HomingType homingType;
    public MMFeedbacks activationFeedback;
    private TargetDetector targetDetector;
    private ClassicMovement movementSystem;
    private Rigidbody2D missileBody;

    public IVessel Owner { get; set; }
    public bool IsActive { get; private set; }
    private void Awake()
    {
        targetDetector = GetComponent<TargetDetector>();
        movementSystem = GetComponent<ClassicMovement>();
        missileBody = GetComponent<Rigidbody2D>();
        
        
        if(!startActive) Deactivate();
    }

    
    void Update()
    {
        if(!IsActive) return;
        
        var target = targetDetector.DetectTarget(Owner);
        if (target == null)
        {
            movementSystem.SetMovement(new Vector2(0, 1));
        }
        else
        {
            if (homingType == HomingType.Simple)
            {
                movementSystem.SetMovement(new Vector2(0, 1));
                movementSystem.SetRotationTo(target.Value);
            }
            else if (homingType==HomingType.Advanced)
            {
                var dirToTarget = (target.Value - missileBody.position).normalized;
                var dirVelocity = missileBody.velocity.normalized;
                var dirCurrent = (Vector2)transform.up;
                
                var targetRotation = dirToTarget - dirVelocity;
                //Газуем только если это не уменьшит наш вектор скорости по отношению к цели
                if (dirVelocity!=Vector2.zero && Vector2.Angle(targetRotation, dirVelocity) < Vector2.Angle(targetRotation, dirCurrent))
                {
                    movementSystem.SetMovement(new Vector2(0,0));
                }
                else
                {
                    movementSystem.SetMovement(new Vector2(0,1));
                }

                movementSystem.SetRotationTo(missileBody.position+targetRotation);
                
                MMDebug.DebugDrawArrow(missileBody.position,targetRotation,Color.magenta);
            }
            
        }

       

        movementSystem.ProcessMovement();
       
       MMDebug.DebugDrawArrow(missileBody.position,missileBody.velocity,Color.red);
       if (target != null)
       {
           MMDebug.DebugDrawArrow(missileBody.position, target.Value - missileBody.position, Color.green);
       }
    }

    public void DelayedActivation(float delay)
    {
        StartCoroutine(processActivation(delay));
    }

    private IEnumerator processActivation(float activationTime)
    {
        yield return new WaitForSeconds(activationTime);
        Activate();
    }

    private void Deactivate()
    {
        IsActive = false;
        Collider2D[] colliders=GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void Activate()
    {
        IsActive = true;
        Collider2D[] colliders=GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        activationFeedback?.PlayFeedbacks();
    }
    
}

public enum HomingType
{
    Simple, Advanced
}
