using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    private MMConeOfVision2D _coneOfVision2D;
    public float minTemperatureToDetect;
    
    private void Awake()
    {
        _coneOfVision2D = GetComponent<MMConeOfVision2D>();
    }

    public Vector2? DetectTarget(IVessel owner)
    {
        _coneOfVision2D.SetDirectionAndAngles(transform.up,Vector3.zero);
        Vector2? target = null;
        float minDistance=float.MaxValue;
        
        for (int i = 0; i < _coneOfVision2D.VisibleTargets.Count; i++)
        {
            if(_coneOfVision2D.VisibleTargets[i]==null || _coneOfVision2D.VisibleTargets[i] == owner.transform) continue;
            //Отсекаем все цели ниже заданной температуры
            var shell = _coneOfVision2D.VisibleTargets[i].GetComponent<Shell>();
            if(shell==null || shell.Temperature < minTemperatureToDetect) continue;
            //Ищем ближайшую цель
            var dist = ((Vector2)(transform.position - _coneOfVision2D.VisibleTargets[i].position)).sqrMagnitude;
            if (dist < minDistance)
            {
                target = _coneOfVision2D.VisibleTargets[i].position;
                minDistance = dist;
            }
        }

        return target;
    }
}
