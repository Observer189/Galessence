using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVessel 
{
    public ShipInfo ShipInfo
    {
        get;
    }
    public PropertyManager PropertyManager { get; }

    public Feeler[] Feelers { get; }

    public VirtualPlayer Owner
    {
        get;
        set;
    }

    public Transform transform { get; }
    
    public bool IsAlive { get; }

    public void Kill()
    {
        
    }
}
