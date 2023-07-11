using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class MiteShortenTheDistanceStrategy : AIStrategy
{
    public override float CalculateApplicability(AIMind mind)
    {
        return 100;
    }

    public override void ApplyStrategy(ShipOrder order, AIMind mind)
    {
        order.movementOrderType = MovementOrderType.RotationTo;
        if (mind.Perception.ClosestShip != null)
        {
            Vector3 moveVec = new Vector3(mind.Perception.ClosestShip.transform.position.x, 0,
                mind.Perception.ClosestShip.transform.position.y);
            if (Vector2.Angle(mind.Perception.ClosestShip.transform.position, mind.Ship.transform.position) < 60)
            {
                moveVec = moveVec.MMSetY(1);
            }


            order.movement = moveVec;
        }
    }
}
