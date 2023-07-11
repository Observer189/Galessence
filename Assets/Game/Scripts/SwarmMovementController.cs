using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts;
using MoreMountains.Tools;
using UnityEngine;

public class SwarmMovementController : MonoBehaviour, IShipActionController
{
    public float movementTargetDistance;
    public float movementTargetRotation;
    public Transform swarmMovementTarget;
    public List<SteeringMovement> dronesMovementSystems;

    public SteeringBehaviorPreset leaderPreset;
    public SteeringBehaviorPreset regularDronePreset;

    public Vector2[] swarmFormationOffsets;
    
    public ShipActionControllerType ControllerType => ShipActionControllerType.MovementController;
    private ShipOrder currentOrder;
    void Start()
    {
        currentOrder = new ShipOrder();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentOrder != null)
        {
            if (currentOrder.movementOrderType == MovementOrderType.Direct)
            {
                /*var moveDir = ((Vector2)transform.up).MMRotate(movementTargetRotation * currentOrder.movement.x);
                moveDir = moveDir.normalized * movementTargetDistance;
                swarmMovementTarget.position = (Vector2)transform.position + moveDir;*/
                
                
                var centerOfMass = Vector2.zero;
                List<Rigidbody2D> droneBodies = new List<Rigidbody2D>();
                foreach (var drone in dronesMovementSystems)
                {
                    if (drone != null)
                    {
                        droneBodies.Add(drone.Body);

                        centerOfMass += drone.Body.position;
                    }
                }

                centerOfMass /= droneBodies.Count;

                var leader = droneBodies[0];

                var formationPositionAssociation = CalculateBestDronesPositions(leader);

                for (int i = 0; i < dronesMovementSystems.Count; i++)
                {
                    var drone = dronesMovementSystems[i];
                    
                    if (drone.Body == leader)
                    {
                        drone.steeringBehaviorPreset = leaderPreset;
                    }
                    else
                    {
                        drone.steeringBehaviorPreset = regularDronePreset;

                        drone.SteeringData.offsetPursuit = swarmFormationOffsets[formationPositionAssociation[i]];
                    }

                    drone.SteeringData.seekTarget = currentOrder.aim;
                    drone.SteeringData.fleeTarget = currentOrder.aim;
                    drone.SteeringData.arriveTarget = currentOrder.aim;

                    drone.SteeringData.neighbors = droneBodies;
                    drone.SteeringData.leader = leader;

                    drone.ProcessMovement();
                }
            }
        }
    }

    int[] CalculateBestDronesPositions(Rigidbody2D leader)
    {
        Vector2[] absoluteOffsetPositions = new Vector2[swarmFormationOffsets.Length];
        Vector2 centerOfMass = Vector2.zero;
        for (int i = 0; i < swarmFormationOffsets.Length; i++)
        {
            absoluteOffsetPositions[i] = leader.position+(Vector2)(leader.transform.up * swarmFormationOffsets[i].y + 
                                                                   leader.transform.right*swarmFormationOffsets[i].x);
            centerOfMass += absoluteOffsetPositions[i];
        }

        centerOfMass /= swarmFormationOffsets.Length;
        //Ключ - расстояние до центра масс
        //Значение - индекс соответствующего дрона в массиве dronesMovementSystems
        SortedDictionary<float, int> distToCenterOfMass = new SortedDictionary<float, int>();
        
        for (int i = 0; i < dronesMovementSystems.Count;i++)
        {
            if (dronesMovementSystems[i].Body != leader)
            {
                distToCenterOfMass.Add(-(dronesMovementSystems[i].Body.position - centerOfMass).sqrMagnitude, i);
            }
        }

        int[] res = new int[dronesMovementSystems.Count];

        HashSet<int> occupiedPositions = new HashSet<int>();
        foreach (var droneInd in distToCenterOfMass.Values)
        {
            
            float minDist = float.MaxValue;
            int minInd = -1;
            for (int i = 0; i < swarmFormationOffsets.Length; i++)
            {
                if (occupiedPositions.Contains(i))
                   continue;

                var dist = (dronesMovementSystems[droneInd].Body.position - absoluteOffsetPositions[i]).sqrMagnitude;
                if (dist<minDist)
                {
                    minDist = dist;
                    minInd = i;
                }
            }

            res[droneInd] = minInd;
            occupiedPositions.Add(minInd);
        }

        return res;
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(20, 30, 200, 200), 
            $"max speed = {dronesMovementSystems[0].MovementSystem.MaxSpeed},\n speed = {dronesMovementSystems[0].GetComponent<Rigidbody2D>().velocity.magnitude}\n" +
            $"calculated acceleration = {dronesMovementSystems[0].MovementSystem.CalculatedAcceleration}\n" +
            $"angular velocity = {dronesMovementSystems[0].GetComponent<Rigidbody2D>().angularVelocity}\n" +
            $"Temperature = {dronesMovementSystems[0].GetComponent<Shell>().Temperature}");
    }
}
