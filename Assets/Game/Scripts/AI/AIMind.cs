using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMind : MonoBehaviour
{
   public bool IsActive=false;

   protected ShipController ship;

   public void SetShip(ShipController s)
   {
      ship = s;
   }

   public void Update()
   {
      if (!IsActive) return;
      ShipOrder order = new ShipOrder();
      order.movement = new Vector2(0, 1);

      ShipController target = null;
      //var mask = LayerMask.NameToLayer("Ships");
      var hit = Physics2D.OverlapCircleAll(ship.transform.position, 30);
      foreach (var col in hit)
      {
         var shipController = col.GetComponent<ShipController>();
         if (shipController != null && shipController != ship)
         {
            target = shipController;
         }
      }

      if (target != null)
      {
         order.movement = target.transform.position - ship.transform.position;
         order.movementIsDirection = true;
         
      }
      //Debug.Log(order.movement);
      ship.UpdateOrder(order);
   }
}
