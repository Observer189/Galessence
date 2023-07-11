using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Preset", menuName = "AI/SteeringBehaviorPreset", order = 3)]
public class SteeringBehaviorPreset : ScriptableObject
{
    public SteeringBehaviorInfo[] workingBehaviors;
}

[Serializable]
public class SteeringBehaviorInfo
{
    public SteeringBehaviorType type;
    public float weight;
}
