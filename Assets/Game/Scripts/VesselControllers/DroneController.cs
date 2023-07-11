using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour, IVessel
{
    public ShipInfo info;

    public ShipInfo ShipInfo => info;
    public PropertyManager PropertyManager { get; }
    public Feeler[] Feelers { get; }
    public VirtualPlayer Owner { get; set; }
    public bool IsAlive { get; }
}
