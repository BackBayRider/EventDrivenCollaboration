using Paramore.Brighter;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Events
{
    public class ShipNameUpdatedEvent : Event
    {
        public Id ShipId { get; }
        public ShipName ShipName { get; }
        public int Version { get; }

        public ShipNameUpdatedEvent(Id shipId, ShipName shipName, int version)
            :base(shipId.Value)
        {
            ShipId = shipId;
            ShipName = shipName;
            Version = version;
        }
    }
}