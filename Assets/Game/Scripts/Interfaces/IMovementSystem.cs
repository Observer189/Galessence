using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementSystem
{
    public float MaxSpeed { get; }

    public float AngularSpeed { get;  }
}
