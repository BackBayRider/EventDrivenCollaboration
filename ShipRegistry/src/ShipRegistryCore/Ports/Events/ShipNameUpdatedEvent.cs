using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Events
{
    public class ShipNameUpdatedEvent : Event
    {
        public Id ShipId { get; }
        public ShipName ShipName { get; }

        public ShipNameUpdatedEvent(Id shipId, ShipName shipName)
            :base(shipId.Value)
        {
            ShipId = shipId;
            ShipName = shipName;
        }
    }
}