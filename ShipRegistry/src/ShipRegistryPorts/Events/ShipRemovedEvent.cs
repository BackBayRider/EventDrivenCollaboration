using System;
using Paramore.Brighter;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Events
{
    public class ShipRemovedEvent : Event
    {
        public Id ShipRemoved { get; }

        public ShipRemovedEvent(Id shipRemoved) 
            : base(Guid.NewGuid())
        {
            ShipRemoved = shipRemoved;
        }
    }
}