using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesCountProperty : ObjectProperty
{
    public ProjectilesCountProperty(float baseValue) : base("Projectiles count", "PC", 11, "ProjectilesCount", 1, 1, baseValue)
    {
    }
}
