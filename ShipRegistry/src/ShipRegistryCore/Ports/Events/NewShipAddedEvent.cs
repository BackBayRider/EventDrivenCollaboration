using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Events
{
    public class NewShipAddedEvent : Event
    {
        public Id ShipId { get; }
        public ShipType Type { get; }
        public ShipName Name { get; }
        public Capacity Capacity { get; }
        public Id ShippingLine { get; }

        public NewShipAddedEvent(Id shipId, ShipType type, ShipName name, Capacity capacity, Id shippingLine) 
            : base(shipId.Value)
        {
            ShipId = shipId;
            Type = type;
            Name = name;
            Capacity = capacity;
            ShippingLine = shippingLine;
        }

     }
}