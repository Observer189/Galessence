using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

//TODO Тест на 2D Тестировалось только при падении, так что при перемещении и ходьбе могут быть ошибки, но не значителеные
public class CameraTracking : MonoBehaviour
{
    protected enum CameraEnum
    {
        Tracking,
        Moving
    }

    public Transform trackingObject;
    [Tooltip("На сколько плавно движется камера")]
    public float smoothSpeed = 0.125f;
    [Header("Axis OX")] [Tooltip("На сколько скорость камеры по ОСИ OX зависит от скорости обьекта")]
    public float velocityMultX = 1;

    [Tooltip("Максимальное смещение по оси OX")]
    public float maxOffsetX = 1;

    [Header("Axis OY")] [Tooltip("На сколько скорость камеры по ОСИ OY зависит от скорости обьекта")]
    public float velocityMultY = 1;

    [Tooltip("Максимальное смещение по оси OY")]
    public float maxOffsetY = 1;

    [SerializeField] protected CameraEnum cameraCondition = CameraEnum.Tracking;
    [Tooltip("Скорость с которой камера движется к новому обьекту")]
    [SerializeField] protected float speedToObject;
    
    protected Vector3 directionToObject;
    protected float posZ;
    protected Vector3 centerOffset;
    protected Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
        posZ = transform.position.z;
        transform.position = new Vector3(trackingObject.position.x, trackingObject.position.y, posZ);
        centerOffset = new Vector2();
    }

    void LateUpdate()
    {
        if(trackingObject == null)
            return; 
        switch (cameraCondition)
        {
            case CameraEnum.Tracking:
                float posX = trackingObject.position.x;
                float posY = trackingObject.position.y;
                var rigidbody = trackingObject.GetComponent<Rigidbody2D>();
                if (rigidbody)
                {
                    centerOffset.x += velocityMultX * rigidbody.velocity.x * Time.deltaTime;
                    centerOffset.x = maxOffsetX < Math.Abs(centerOffset.x)
                        ? (centerOffset.x > 0 ? maxOffsetX : -maxOffsetX)
                        : centerOffset.x;
                    posX += centerOffset.x;
                    centerOffset.y += velocityMultY * rigidbody.velocity.y * Time.deltaTime;
                    centerOffset.y = maxOffsetY < Math.Abs(centerOffset.y)
                        ? (centerOffset.y > 0 ? maxOffsetY : -maxOffsetY)
                        : centerOffset.y;
                    posY += centerOffset.y;
                }
                var smoothedPosition =  Vector3.Lerp(transform.position, new Vector3(posX, posY, posZ), smoothSpeed);
               
                transform.position = smoothedPosition;
                break;
            case CameraEnum.Moving: //Плавное перемещение к новому обьекту
                if (Vector3.Distance(transform.position, trackingObject.position) < speedToObject)
                {
                    cameraCondition = CameraEnum.Tracking;
                    transform.position = trackingObject.position;
                    directionToObject = Vector2.zero;
                }
                else
                    transform.position += directionToObject * Time.deltaTime;
                break;
        }
    }

    public void SetTrackingObject(Transform trackingObject)
    {
        this.trackingObject = trackingObject;
        directionToObject = (trackingObject.position - transform.position).normalized ;
        centerOffset = Vector2.zero;
        cameraCondition = CameraEnum.Moving;
    }

    public void SetZoom(float Zoom)
    {
        if (camera.orthographic)
            camera.orthographicSize = Zoom;
    }
}
