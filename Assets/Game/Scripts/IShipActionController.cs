namespace Game.Scripts
{
    public interface IShipActionController
    {
        public void UpdateOrder(ShipOrder order);

        public ShipActionControllerType ControllerType { get; }
    }

    public enum ShipActionControllerType
    {
     MovementController, WeaponController    
    }
}