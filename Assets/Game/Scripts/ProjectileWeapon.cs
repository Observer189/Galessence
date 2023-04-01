using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour, IShipActionController
{
    public Transform forwardDir;
    public Transform projectileSpawnPosition;

    public GameObject projectilePrefab;

    public float reloadTime;

    public float projectileSpeed;

    public float baseDamage;

    public EffectDescription[] effects;

    public Vector2 knockBackForce;

    public bool addShipVelocity;

    protected bool isLoaded;
    protected float lastShotTime;
    protected Shell shell;
    protected Rigidbody2D body;
    protected ObjectProperty damage;
    protected PropertyManager _propertyManager;

    protected ShipOrder currentOrder;
    protected ShipController controller;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        shell = GetComponent<Shell>();
        controller = GetComponent<ShipController>();
        _propertyManager = GetComponent<PropertyManager>();
    }

    void Start()
    {
        damage = _propertyManager.AddProperty("PhysicalDamage", baseDamage);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastShotTime > reloadTime)
        {
            isLoaded = true;
        }

        if (currentOrder != null)
        {
            if (currentOrder.mainWeapon && isLoaded)
            {
                Shot();
            }
        }
    }

    public void Shot()
    {
        var proj = Instantiate(projectilePrefab, projectileSpawnPosition.position, forwardDir.rotation).GetComponent<Projectile>();
        //body.AddForce(knockBackForce.y*forwardDir.up + knockBackForce.x*forwardDir.right,ForceMode2D.Impulse);
        proj.EffectOnTouch.SetOwner(controller);
        for (int i = 0; i < effects.Length; i++)
        {
            proj.EffectOnTouch.AddEffect(new Effect(effects[i],_propertyManager));
        }
       
        var projSpeed = projectileSpeed* (Vector2)proj.transform.up;
        if (addShipVelocity)
        {
            projSpeed += body.velocity;
        }

        proj.Shell.AddImpulse(proj.Shell.Body.mass*projSpeed);
        
        shell.AddImpulse(knockBackForce.y*forwardDir.up + knockBackForce.x*forwardDir.right);
        lastShotTime = Time.time;
        isLoaded = false;
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

    public ShipActionControllerType ControllerType { get; }
}
