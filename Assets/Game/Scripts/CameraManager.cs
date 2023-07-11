using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MoreMountains.Tools;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

public class CameraManager : MMSingleton<CameraManager>
{
    public Transform AuxilaryTransform;
    public float zoomSpeed = 0.5f;
    [Tooltip("Определяет как сильно движение курсора будет влиять на движение камеры")]
    public float cursorFollowCoef = 0.3f;
    
    public float minCameraSize=1.5f;
    public float maxCameraSize=10f;
    [Tooltip("Скорость движения камеры в свободном режиме")]
    public float freeModeSpeed = 3f;
    [Tooltip("Определяет какой процент от размера экрана нужно отступить от края, чтобы считалось, что мышка подведена к краю" +
             "и соотвественно нужно двигать экран в нужную страну")]
    public float freeModeThresholdToMoveByMouse = 0.15f;
    protected CinemachineVirtualCamera virtualCamera;
    protected Camera mainCamera;

    protected CameraMode mode;

    protected Transform target1;
    protected Transform target2;

    protected Transform followTargetInFreeMode;

    protected CameraOrder currentOrder;

    public CameraMode Mode => mode;
    protected override void Awake()
    {
        base.Awake();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        
    }

    /*public void SetFollowTarget(Transform target)
    {
        virtualCamera.LookAt = target;
        virtualCamera.Follow = target;

        mode = CameraMode.OneTarget;
    }

    public void SetFollowTwoTargets(Transform target1, Transform target2)
    {
        this.target1 = target1;
        this.target2 = target2;

        //virtualCamera.Follow = AuxilaryTransform;
        //virtualCamera.LookAt = AuxilaryTransform;

        mode = CameraMode.TwoTarget;
    }*/

    private void Update()
    {
        /*if (mode == CameraMode.TwoTarget)
        {
            AuxilaryTransform.position = (target1.position + target2.position) / 2;
            Debug.Log($"y = {Mathf.Abs(target1.position.y - target2.position.y)}, x= {Mathf.Abs(target1.position.x - target2.position.x) / virtualCamera.m_Lens.Aspect}");
            Debug.Log(virtualCamera.m_Lens.Orthographic);
            virtualCamera.m_Lens.OrthographicSize = Mathf.Max(Mathf.Abs(target1.position.y - target2.position.y)/2,
                Mathf.Abs(target1.position.x - target2.position.x) / (virtualCamera.m_Lens.Aspect * 2) );
        }*/
        if (mainCamera == null)
        {
            mainCamera=Camera.main;
        }

        //Debug.Log(mode);
        switch (mode)
        {
            case CameraMode.Off:
                break;
            case CameraMode.OneTarget:
                HandleOneTarget();
                break;
            case CameraMode.TwoTarget:
                HandleTwoTarget();
                break;
            case CameraMode.Free:
                HandleFreeMode();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    protected void HandleOneTarget()
    {
        if(target1==null) return;
        Vector2 posOffset = Vector2.zero;
        
        if (currentOrder != null)
        {
            virtualCamera.m_Lens.OrthographicSize += currentOrder.zoom * zoomSpeed*Time.deltaTime;
            var cursorPosition = mainCamera.ScreenToWorldPoint(currentOrder.mouseAim);
            posOffset += (Vector2)(cursorPosition - target1.position) * cursorFollowCoef;
        }
        
        AuxilaryTransform.position =(Vector2)target1.position+posOffset;
        
        virtualCamera.m_Lens.OrthographicSize =
            Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize, minCameraSize, maxCameraSize);
    }

    protected void HandleTwoTarget()
    {
        if (target1 != null && target2 != null)
        {
            AuxilaryTransform.position = (target1.position + target2.position) / 2;
            float distanceBetweenTargets = Vector2.Distance(target1.position, target2.position);

            // Calculate the furthest distance from the center
            float maxDistanceFromCenter = Mathf.Max(Mathf.Abs(target1.position.x - AuxilaryTransform.position.x),
                Mathf.Abs(target2.position.x - AuxilaryTransform.position.x),
                Mathf.Abs(target1.position.y - AuxilaryTransform.position.y),
                Mathf.Abs(target2.position.y - AuxilaryTransform.position.y));

            // Use Pythagoras' theorem to calculate the required orthographic size
            float requiredSize =
                Mathf.Sqrt(Mathf.Pow(maxDistanceFromCenter, 2) + Mathf.Pow(distanceBetweenTargets / 2f, 2));
            if (requiredSize < maxCameraSize)
            {
                virtualCamera.Follow = AuxilaryTransform;
                virtualCamera.LookAt = AuxilaryTransform;
                // Set the camera's orthographic size to the required size
                virtualCamera.m_Lens.OrthographicSize = requiredSize;
            }
            else
            {
                HandleOneTarget();
            }
        }
        else if (target1!=null && target2 == null)
        {
            HandleOneTarget();
        }
    }

    protected void HandleFreeMode()
    {
        if (currentOrder != null)
        {

            virtualCamera.m_Lens.OrthographicSize += currentOrder.zoom * zoomSpeed*Time.deltaTime;
            var keyBoardMovement = (Vector3)currentOrder.movement.normalized * freeModeSpeed * Time.deltaTime;
            if (currentOrder.movement.magnitude > 0.1f)
            {
                followTargetInFreeMode = null;
            }

            #region MovementByMouse

            var xThresholdSize = Screen.width * freeModeThresholdToMoveByMouse;
            var yThresholdSize = Screen.height * freeModeThresholdToMoveByMouse;

            var mouseMovement = Vector2.zero;
            //Если подводим мышку к краю экрана
            if (currentOrder.mouseAim.x < xThresholdSize || currentOrder.mouseAim.x > Screen.width - xThresholdSize
                                                         || currentOrder.mouseAim.y < yThresholdSize ||
                                                         currentOrder.mouseAim.y > Screen.height - yThresholdSize)
            {
                mouseMovement = -(new Vector2((Screen.width/2-currentOrder.mouseAim.x)/Screen.width,
                    (Screen.height/2-currentOrder.mouseAim.y)/Screen.height).normalized);
                mouseMovement *= freeModeSpeed * Time.deltaTime;
            }


            #endregion
            
            AuxilaryTransform.position += keyBoardMovement + (Vector3)mouseMovement;
            //Debug.Log( keyBoardMovement + (Vector3)mouseMovement);
        }
        
        virtualCamera.m_Lens.OrthographicSize =
            Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize, minCameraSize, maxCameraSize);

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit != null && hit.collider.attachedRigidbody!=null 
                            && hit.collider.attachedRigidbody.GetComponent<IVessel>()!=null)
            {
                followTargetInFreeMode = hit.collider.attachedRigidbody.transform;
            }
        }
        
        if (followTargetInFreeMode !=null)
        {
            target1 = followTargetInFreeMode;
            HandleOneTarget();
        }
        
    }

    public void UpdateCameraOrder(CameraOrder order)
    {
        currentOrder = order;
        if (currentOrder != null)
        {
         
            if (currentOrder.changeMode)
            {
                mode = currentOrder.newMode;
                target1 = currentOrder.target1;
                target2 = currentOrder.target2;
            }
        }
    }
}

public enum CameraMode
{
    Off, OneTarget, TwoTarget, Free
}
