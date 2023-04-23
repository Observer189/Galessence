using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrder
{
   public bool changeMode;
   public CameraMode newMode;
   public float zoom;
   public Vector2 movement;
   public Vector2 mouseAim;
   public Transform target1;
   public Transform target2;

   public CameraOrder()
   {
      
   }
}
