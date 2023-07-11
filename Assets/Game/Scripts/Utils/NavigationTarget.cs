using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class NavigationTarget : MonoBehaviour
{
   public MMFeedbacks onTriggerShipFeedback;

   private void OnTriggerEnter2D(Collider2D col)
   {
      if (col.GetComponent<IVessel>() != null)
      {
        
         onTriggerShipFeedback?.PlayFeedbacks();
      }
   }
}
