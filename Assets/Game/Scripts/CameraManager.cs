using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MoreMountains.Tools;
using UnityEngine;

public class CameraManager : MMSingleton<CameraManager>
{
    protected CinemachineVirtualCamera virtualCamera;
    protected override void Awake()
    {
        base.Awake();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetFollowTarget(Transform target)
    {
        virtualCamera.LookAt = target;
        virtualCamera.Follow = target;
    }
}
