using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AsteroidDragger : MonoBehaviour
{
    public LayerMask dragMask;
    public float dragAcceleration = 0.5f;
    private Camera mainCamera;
    private Rigidbody2D draggedBody;
    private Vector2 lastMousePos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.OverlapPoint(mainCamera.ScreenToWorldPoint(Input.mousePosition),dragMask);
            if (hit != null)
            {
                var body = hit.attachedRigidbody;
                if (body != null)
                {
                    draggedBody = body;
                    lastMousePos = Input.mousePosition;
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            draggedBody = null;
        }

        var mouseDelta = (Vector2)Input.mousePosition - lastMousePos;
        draggedBody?.AddForce(mouseDelta*draggedBody.mass*dragAcceleration*Time.deltaTime,ForceMode2D.Impulse);

        lastMousePos = Input.mousePosition;

    }
}
