using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiteUseLaserStrategy : AIStrategy
{
    public override float CalculateApplicability(AIMind mind)
    {
        if (mind.Ship.PropertyManager.GetPropertyByName("Energy").GetCurValue() <= 0)
        {
            return 0;
        }

        var laser = mind.Ship.transform.GetComponent<LaserWeapon>();
        var hits = Physics2D.RaycastAll(laser.BeamStartPosition.position,laser.BeamStartPosition.up,laser.BeamRange,laser.BeamTargetMask);
        
        if (hits.Length > 0)
        {
            //Убираем все столкновения с собой чтобы не перекрывать дорогу лазеру
            hits = hits.Where((RaycastHit2D h) => h.collider != laser.GetComponent<Collider2D>()).ToArray();
        }
        
        if (hits.Length > 0)
        {
            var ship = hits[0].rigidbody.GetComponent<IVessel>();
            if (ship != null && ship.Owner.team.number != mind.Ship.Owner.team.number)
            {
                return 100;
            }
        }

        return 0;
    }

    public override void ApplyStrategy(ShipOrder order, AIMind mind)
    {
        order.mainWeapon = true;
    }
}
