namespace ShipRegistryCore.Application
{
    public class Ship : IEntity
    {
        public Id Id { get; set; }
        public ShipName ShipName { get; set; }
        public ShipType ShipType { get; set; }
        public Capacity Capacity { get; set; }
        public Id ShippingLineId { get; set; }
        
        
        public Ship(Id id, ShipName shipName, ShipType shipType, Capacity capacity, Id shippingLineId)
        {
            Id = id;
            ShipName = shipName;
            ShipType = shipType;
            Capacity = capacity;
            ShippingLineId = shippingLineId;
        }
    }
}