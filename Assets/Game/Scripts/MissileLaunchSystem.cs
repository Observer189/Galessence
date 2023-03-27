using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts;
using UnityEngine;

public class MissileLaunchSystem : MonoBehaviour, IShipActionController
{
    [Tooltip("Минимальная пауза между запусками, которая используется даже если все ракеты заряжены")]
    public float timeBetweenLaunches;
    public float reloadTime;
    public float missileActivationDelay;
    public MissileLauncher[] launchers;

    private int robinCounterLaunch = 0;
    private int robinCounterReload = 0;

    private float lastLaunchTime;
    private float lastReloadTime;

    private Rigidbody2D shipBody;
    
    private ShipOrder currentOrder;

    private void Awake()
    {
        shipBody = GetComponent<Rigidbody2D>();
    }

    public void LaunchMissile()
    {
        if (launchers[robinCounterLaunch].IsLoaded && (Time.time - lastLaunchTime) > timeBetweenLaunches)
        {
            launchers[robinCounterLaunch].LaunchMissile(shipBody.velocity,missileActivationDelay);

            lastLaunchTime = Time.time;
            robinCounterLaunch++;
            if (robinCounterLaunch >= launchers.Length)
                robinCounterLaunch = 0;
        }
    }

    private void Update()
    {
        if (!launchers[robinCounterReload].IsLoaded && Mathf.Min(Time.time - lastLaunchTime, Time.time - lastReloadTime) > reloadTime)
        {
            launchers[robinCounterReload].LoadMissile();
            robinCounterReload++;
            if (robinCounterReload >= launchers.Length)
            robinCounterReload = 0;
            lastReloadTime = Time.time;
        }
        
        if (currentOrder!=null && currentOrder.secondaryWeapon)
        {
            LaunchMissile();
        }
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

    public ShipActionControllerType ControllerType => ShipActionControllerType.WeaponController;
}
