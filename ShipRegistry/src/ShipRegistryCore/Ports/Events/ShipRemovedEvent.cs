using System;
using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Events
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