using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;


public class GravityCircle : MonoBehaviour
{
    [SerializeField]
    protected float smallRadius;
    [SerializeField]
    protected float bigRadius;
    [SerializeField]
    protected float minForce;
    [SerializeField]
    protected float maxForce;
    
    protected Material material; 

    public float SmallRadius
    {
        get => smallRadius;
        set
        {
            smallRadius = value;
            material.SetFloat("_InnerRadius", smallRadius);
        }
    }

    public float BigRadius
    {
        get => bigRadius;
        set
        {
            bigRadius = value;
            transform.localScale = new Vector3(bigRadius*2, bigRadius*2, 0);
        }
    }

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        
    }

    void Start()
    {
        BigRadius = bigRadius;
        SmallRadius = smallRadius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.DrawWireSphere(transform.position,smallRadius);
        Gizmos.DrawWireSphere(transform.position,bigRadius);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //Вычисляем расстояние от объекта до центра круга
        var objRadius = ((Vector2)transform.position - other.attachedRigidbody.position).SqrMagnitude();
        //Если объект вне малого радиуса, то игнорируем его
        if(objRadius < smallRadius * smallRadius) return;
        
        var shell = other.attachedRigidbody.GetComponent<Shell>();

        if (shell != null)
        {
            objRadius = Mathf.Sqrt(objRadius);
            var impulseMagnitude = (objRadius - smallRadius)/(bigRadius-smallRadius)*(maxForce-minForce) + minForce;
            var dir = (Vector2)transform.position - other.attachedRigidbody.position.normalized;
            //Debug.Log(impulseMagnitude);
            shell.AddImpulse(dir*impulseMagnitude);
        }
    }
}
