using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPerception
{
    protected List<IVessel> enemyShips;

    protected IVessel closestShip;

    public Transform navigationTarget;
    public List<IVessel> EnemyShips => enemyShips;

    public IVessel ClosestShip => closestShip;

    public void UpdateInfo(IVessel ship)
    {

        enemyShips = new List<IVessel>();
        
        var hit = Physics2D.OverlapCircleAll(ship.transform.position, 60, LayerMask.GetMask("Ships"));
        foreach (var col in hit)
        {
            float minDist = float.MaxValue;
            var s = col.GetComponent<IVessel>();
            if (s != null && s.Owner.team.number != ship.Owner.team.number)
            {
                enemyShips.Add(s);
                var dist = Vector2.SqrMagnitude(ship.transform.position - s.transform.position);
                if (dist < minDist)
                {
                    closestShip = s;
                    minDist = dist;
                }
            }
        }

        var h = Physics2D.OverlapCircle(ship.transform.position, 600, LayerMask.GetMask("Default"));
        if (h != null && h.GetComponent<NavigationTarget>() != null)
        {
            navigationTarget = h.transform;
        }
    }
    
    
}
