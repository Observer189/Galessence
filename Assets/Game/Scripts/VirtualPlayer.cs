using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualPlayer : MonoBehaviour
{
    public string name;
    public PlayerType playerType;
    public ShipController ship;
    
    protected ShipOrder currentOrder;

    private void Update()
    {
        if (ship != null)
        {
            if (ship.Owner == null)
            {
                ship.Owner = this;
                CameraManager.Instance.SetFollowTarget(ship.cameraTarget);
            }

            ship.UpdateOrder(currentOrder?.GetCopy());
        }
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

}

public enum PlayerType
{
    Player,AI,RemotePlayer
}
