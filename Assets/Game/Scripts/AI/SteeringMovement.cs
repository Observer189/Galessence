using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Utils;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;

public class SteeringMovement : MonoBehaviour
{
    public SteeringBehaviorPreset steeringBehaviorPreset;
    public LayerMask obstacleMask;
    
    protected Rigidbody2D body;
    
    protected ClassicMovement movementSystem;
    
    protected SteeringMovementData steeringData;

    public ClassicMovement MovementSystem => movementSystem;
    public Rigidbody2D Body => body;
    public SteeringMovementData SteeringData => steeringData;

    protected delegate Vector2 SteeringBehavior(SteeringMovementData data, IMovementSystem movementSystem, Rigidbody2D body);

    private void Awake()
    {
        steeringData = new SteeringMovementData();
        body = GetComponent<Rigidbody2D>();
        movementSystem = GetComponent<ClassicMovement>();
    }

    public void ProcessMovement()
    {
        Vector2 accumulatedDesireVelocity = Vector2.zero;

        foreach (var behavior in steeringBehaviorPreset.workingBehaviors)
        {
            accumulatedDesireVelocity+= 
                AssociateSteeringBehavior(behavior.type).Invoke(this.steeringData,this.movementSystem,this.body) * behavior.weight;
        }

        accumulatedDesireVelocity = accumulatedDesireVelocity.normalized *
                                    Mathf.Min(accumulatedDesireVelocity.magnitude, movementSystem.MaxSpeed);
        //Debug.Log("Source: "+accumulatedDesireVelocity);
        var verticalSpeed = accumulatedDesireVelocity.ProjectionTo(body.transform.up);
        var horizontalSpeed = accumulatedDesireVelocity.ProjectionTo(body.transform.right);
        
        movementSystem.SetMovement(new Vector4(0,verticalSpeed,horizontalSpeed,0));
        movementSystem.SetRotationTo(steeringData.arriveTarget);
        movementSystem.CalculateAngularDesireSpeedByRotationTo();
        movementSystem.SetTargetSpeedMode(true);
        
        movementSystem.ProcessMovement();
    }
    
    protected SteeringBehavior AssociateSteeringBehavior(SteeringBehaviorType type)
    {
        switch (type)
        {
            case SteeringBehaviorType.Seek:
                return Seek;
            case SteeringBehaviorType.Flee:
                return Flee;
            case SteeringBehaviorType.Arrive:
                return Arrive;
            case SteeringBehaviorType.Pursuit:
                return Pursuit;
            case SteeringBehaviorType.Separating:
                return Separating;
            case SteeringBehaviorType.Cohesion:
                return Cohesion;
            case SteeringBehaviorType.ObstacleAvoidance:
                return ObstacleAvoidance;
            case SteeringBehaviorType.OffsetPursuit:
                return OffsetPursuit;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        return null;
    }

    protected Vector2 Seek(SteeringMovementData data, IMovementSystem movementSystem, Rigidbody2D body)
    {
        return SeekCalc(data.seekTarget, movementSystem, body);
    }

    protected Vector2 SeekCalc(Vector2 targetPos, IMovementSystem movementSystem, Rigidbody2D body)
    {
        return (targetPos - body.position).normalized * movementSystem.MaxSpeed;
    }

    protected Vector2 Flee(SteeringMovementData data, IMovementSystem movementSystem, Rigidbody2D body)
    {
        return (body.position - data.fleeTarget).normalized * movementSystem.MaxSpeed;
    }

    protected Vector2 Arrive(SteeringMovementData data, IMovementSystem movementSystem, Rigidbody2D body)
    {
        return ArriveCalc(data.arriveTarget, 0.5f, movementSystem, body);
    }

    protected Vector2 ArriveCalc(Vector2 targetPos, float decelerationSpeed, IMovementSystem movementSystem, Rigidbody2D body)
    {
        Vector2 toTarget = targetPos - body.position;

        var dist = toTarget.magnitude;

        if (dist > 0)
        {
            float speed = dist / decelerationSpeed;

            speed = Mathf.Min(speed, movementSystem.MaxSpeed);

            return toTarget * speed / dist;
        }
        
        return Vector2.zero;
    }

    protected Vector2 Pursuit(SteeringMovementData data, IMovementSystem movementSystem, Rigidbody2D body)
    {
        Vector2 toEvader = data.pursuitTarget.position - body.position;

        float relativeHeading = Vector2.Dot(body.transform.up, data.pursuitTarget.transform.up);

        if (Vector2.Dot(toEvader, data.pursuitTarget.transform.up) > 0 && relativeHeading < 0.95)
        {
            return SeekCalc(data.pursuitTarget.position, movementSystem,body);
        }

        float lookAheadTime = toEvader.magnitude / (movementSystem.MaxSpeed + data.pursuitTarget.velocity.magnitude);

        return SeekCalc(data.pursuitTarget.position + data.pursuitTarget.velocity * lookAheadTime
        ,movementSystem,body);
    }

    protected Vector2 Separating(SteeringMovementData data, IMovementSystem movementSystem, Rigidbody2D body)
    {
        Vector2 desireVelocity = Vector2.zero;

        foreach (var neighbor in data.neighbors)
        {
            if (neighbor != body)
            {
                var toAgent = body.position - neighbor.position;

                desireVelocity += toAgent.normalized / toAgent.magnitude;
            }
        }

        return desireVelocity;
    }

    protected Vector2 Cohesion(SteeringMovementData data, IMovementSystem movementSystem, Rigidbody2D body)
    {
        Vector2 centerOfMass = Vector2.zero;
        Vector2 desireVelocity = Vector2.zero;

        foreach (var neighbor in data.neighbors)
        {
            if (neighbor != this.body)
            {
                centerOfMass += neighbor.position;
            }
            
            
        }

        if (data.neighbors.Count > 0)
        {
            centerOfMass /= data.neighbors.Count;

            desireVelocity = SeekCalc(centerOfMass,movementSystem,body);
        }

        return desireVelocity;
    }

    protected Vector2 ObstacleAvoidance(SteeringMovementData data, IMovementSystem movementSystem, Rigidbody2D body)
    {
        var shipWidth = 0.6f;
        var shipLength = 0.6f;
        var minDetectionBoxLength = 2f;
        var m_dBoxLength = minDetectionBoxLength +
                           (body.velocity.magnitude / movementSystem.MaxSpeed) * minDetectionBoxLength;
        
    
        var velDir = body.velocity.normalized;
        var perpVelDir = velDir.MMRotate(-90);

        
        
        var hit = Physics2D.BoxCast(body.position,
            new Vector2(shipLength, shipWidth), Vector2.SignedAngle(Vector2.right, body.velocity), body.velocity,
            m_dBoxLength,obstacleMask);
        Debug.Log(hit.collider);
        /*var obstacles = Physics2D.OverlapAreaAll(body.position + perpVelDir * shipwidth/2,
            body.position + velDir * body.velocity.magnitude * 1.5f - perpVelDir * shipwidth/2,LayerMask.NameToLayer("Asteroids"));

        if (obstacles != null)
        {
            Collider2D closestObstacle = obstacles[0];
            var minDist = (closestObstacle.attachedRigidbody.position - body.position).magnitude;
            var headDist = toObstacle.ProjectionTo(velDir);

            for (int i = 0; i < obstacles.Length; i++)
            {
                var dist = (obstacles[i].attachedRigidbody.position - body.position).magnitude;
                var hd = (closestObstacle.attachedRigidbody.position - body.position).ProjectionTo(velDir);
                if (hd > 0 && dist < minDist)
                {
                    closestObstacle = obstacles[i];
                    minDist = dist;
                }
            }*/
        var desireVelocity = Vector2.zero;
        if (hit)
        {

            var toObstacle = hit.collider.attachedRigidbody.position - body.position;

            var headDist = toObstacle.ProjectionTo(velDir);
            var perpHeadDist = toObstacle.ProjectionTo(perpVelDir);

            var obstacleRadius =
                (hit.collider.attachedRigidbody.position - hit.point)
                .magnitude; //(hit.collider.bounds.max - hit.collider.bounds.min).magnitude;
            MMDebug.DrawPoint(hit.collider.attachedRigidbody.position,Color.white, obstacleRadius);
            Debug.Log("Radius = "+obstacleRadius);
            float multiplier = 1.5f + (m_dBoxLength - headDist) / m_dBoxLength;
            Debug.Log("Multiplier = "+multiplier);
            Debug.Log("Y = "+(obstacleRadius - perpHeadDist)*multiplier);
            //SteeringForce.MMSetY((obstacleRadius - perpHeadDist)*multiplier);

            var brakingWeight = 0.6f;

           // SteeringForce.MMSetX((obstacleRadius - headDist)*brakingWeight);
           var laterVelocity = ((obstacleRadius - perpHeadDist) * multiplier) * perpVelDir;
           var brakingVelocity = (obstacleRadius - headDist) * brakingWeight * velDir;
           desireVelocity =brakingVelocity + laterVelocity;
            
            //Debug.Log(SteeringForce.x);
        }

        
        MMDebug.DebugDrawArrow(transform.position,desireVelocity,Color.yellow);
        return desireVelocity;
    }

    protected Vector2 OffsetPursuit(SteeringMovementData data, IMovementSystem movementSystem, Rigidbody2D body)
    {
        return OffsetPursuitCalc(data.leader, data.offsetPursuit, movementSystem, body);
    }

    protected Vector2 OffsetPursuitCalc(Rigidbody2D leader, Vector2 offset, IMovementSystem movementSystem, Rigidbody2D body)
    {
        
        Vector2 worldOffsetPos = leader.position+(Vector2)(leader.transform.up * offset.y + leader.transform.right*offset.x);

        MMDebug.DrawPoint(worldOffsetPos,Color.red, 0.05f);
        
        Vector2 toOffset = worldOffsetPos - body.position;

        float lookAheadTime = toOffset.magnitude / (movementSystem.MaxSpeed + leader.velocity.magnitude);

        return ArriveCalc(worldOffsetPos + leader.velocity * lookAheadTime, 0.5f, movementSystem, body);
    }
}

public class SteeringMovementData
{
    public Vector2 seekTarget;
    public Vector2 fleeTarget;
    public Vector2 arriveTarget;

    public Rigidbody2D pursuitTarget;

    public List<Rigidbody2D> neighbors;

    public List<Collider2D> obstacles;

    public Vector2 offsetPursuit;

    public Rigidbody2D leader;
}


public enum SteeringBehaviorType
{
    Seek,Flee, Arrive, Pursuit, ObstacleAvoidance, Separating, Cohesion, OffsetPursuit
}
