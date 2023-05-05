using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class NavigationTargetStrategy : AIStrategy
{
    
    public override float CalculateApplicability(AIMind mind)
    {
        return 100;
    }

    public override void ApplyStrategy(ShipOrder order, AIMind mind)
    {
        order.movementHasRotationDirection = true;
        if (mind.Perception.navigationTarget != null)
        {
            Vector3 moveVec = new Vector3(mind.Perception.navigationTarget.position.x, 0,
                mind.Perception.navigationTarget.position.y);
            if (Vector2.Angle(mind.Perception.navigationTarget.position-mind.Ship.transform.position, mind.Ship.transform.up) < 15)
            {
                moveVec = moveVec.MMSetY(1);
            }


            order.movement = moveVec;
        }
    }
}
