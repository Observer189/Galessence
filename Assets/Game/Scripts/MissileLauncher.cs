using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    public GameObject missilePrefab;
    [Tooltip("Точка к которой будет крепиться ракета. Ракета также будет развернута туда же, куда и точка крепления")]
    public Transform missileFixPosition;

    public Vector2 missileLaunchDir;
    public float missileLaunchVelocity;
    
    
    public bool IsLoaded => currentMissile != null;

    private MissileController currentMissile = null;
    public void LoadMissile()
    {
        if(currentMissile!=null) return;
        currentMissile = Instantiate(missilePrefab, missileFixPosition.position, missileFixPosition.rotation,
            missileFixPosition).GetComponent<MissileController>();
        currentMissile.GetComponent<Rigidbody2D>().simulated = false;
    }

    public void LaunchMissile(Vector2 shipVelocity,float missileActivationDelay)
    {
        if(currentMissile==null) return;

        currentMissile.Owner = GetComponent<ShipController>();
        currentMissile.transform.parent = null;
        currentMissile.DelayedActivation(missileActivationDelay);
        
        var missileBody = currentMissile.GetComponent<Rigidbody2D>();
        missileBody.simulated = true;
        Vector2 launchDir = missileFixPosition.up * missileLaunchDir.x + missileFixPosition.right * missileLaunchDir.y;
        missileBody.AddForce((launchDir.normalized*missileLaunchVelocity+shipVelocity)*missileBody.mass,ForceMode2D.Impulse);

        currentMissile = null;
    }
}
