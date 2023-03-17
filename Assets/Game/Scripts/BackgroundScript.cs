using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    public Transform camera;

    public float speed;//скорость слоя относительно скорости камеры

    private Vector2 cameraOldPosition;
    void Start()
    {
        cameraOldPosition = camera.position;
    }

    // Update is called once per frame
   

    private void LateUpdate()
    {
        var delta = (Vector2)camera.position - cameraOldPosition;
        transform.Translate(delta*speed);
        cameraOldPosition = camera.position;
    }
}
