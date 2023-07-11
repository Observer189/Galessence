using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class Feeler : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public float feelDistance;

    private Transform feelTarget;
    private float distanceToTarget;

    public Transform FeelTarget => feelTarget;

    public float DistanceToTarget => distanceToTarget;
    
    // Update is called once per frame
    void Update()
    {
        var hit = Physics2D.Raycast(transform.position,transform.up,feelDistance,obstacleLayer);
        if (hit != null)
        {
            feelTarget = hit.transform;
            distanceToTarget = hit.distance;
        }
    }

    private void OnDrawGizmos()
    {
        var color = Gizmos.color;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position,transform.position+transform.up*feelDistance);
        Gizmos.color = color;
    }
}
