using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualPlayer : MonoBehaviour
{
    public string name;
    public PlayerType playerType;
    public Team team;
    public ShipController ship;
    
    protected ShipOrder currentOrder;
    protected CameraOrder currentCameraOrder;

    private void Start()
    {
        if (playerType == PlayerType.AI)
        {
            Debug.Log(ship.ShipInfo.AIMindPrefab);
            Instantiate(ship.ShipInfo.AIMindPrefab,transform).GetComponent<AIMind>().SetShip(ship);
        }

        if (ship != null)
        {
            ship.Owner = this;
            if (playerType == PlayerType.Player)
            {
                //CameraManager.Instance.SetFollowTarget(ship.cameraTarget);
                var ord = new CameraOrder();
                ord.changeMode = true;
                ord.target1 = ship.CameraTarget;
                ord.newMode = CameraMode.OneTarget;
                CameraManager.Instance.UpdateCameraOrder(ord);
            }
        }
    }

    private void Update()
    {
        if (ship != null)
        {
            if (ship.Owner == null)
            {
                ship.Owner = this;
                if (playerType == PlayerType.Player)
                {
                    //CameraManager.Instance.SetFollowTarget(ship.cameraTarget);
                }
            }
            ship.UpdateOrder(currentOrder?.GetCopy());
        }

        if (playerType == PlayerType.Player && currentCameraOrder!=null)
        {
            if (ship != null)
            {
                if (currentCameraOrder.changeMode)
                {
                    if (CameraManager.Instance.Mode != CameraMode.OneTarget)
                    {
                        currentCameraOrder.newMode = CameraMode.OneTarget;
                    }
                    else if (CameraManager.Instance.Mode != CameraMode.TwoTarget)
                    {
                        currentCameraOrder.newMode = CameraMode.TwoTarget;
                    }
                }

                currentCameraOrder.target1 = ship.transform;

                if (currentCameraOrder.newMode == CameraMode.TwoTarget ||
                    CameraManager.Instance.Mode == CameraMode.TwoTarget)
                {
                    IVessel closestShip = null;
                    var hit = Physics2D.OverlapCircleAll(ship.transform.position, 60, LayerMask.GetMask("Ships"));
                    foreach (var col in hit)
                    {
                        float minDist = float.MaxValue;
                        var s = col.GetComponent<IVessel>();
                        if (s != null && s.Owner.team.number != ship.Owner.team.number)
                        {
                            var dist = Vector2.SqrMagnitude(ship.transform.position - s.transform.position);
                            if (dist < minDist)
                            {
                                closestShip = s;
                                minDist = dist;
                            }
                        }
                    }
                    if(closestShip!=null)
                    currentCameraOrder.target2 = closestShip.transform;
                }
                
            }
            else
            {
                if (CameraManager.Instance.Mode != CameraMode.Free)
                {
                    currentCameraOrder.changeMode = true;
                    currentCameraOrder.newMode = CameraMode.Free;
                }
            }
            CameraManager.Instance.UpdateCameraOrder(currentCameraOrder);
        }
    }

    public void UpdateOrder(ShipOrder order)
    {
        currentOrder = order;
    }

    public void UpdateCameraOrder(CameraOrder order)
    {
        currentCameraOrder = order;
    }

}
[Serializable]
public class Team
{
    public int number;
    public Color color;
}

public enum PlayerType
{
    Player,AI,RemotePlayer
}
