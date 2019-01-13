using System;
using Paramore.Brighter;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Commands
{
    public class RemoveShipCommand : Command
    {
        public Id ShipId { get; }

        public RemoveShipCommand(Id shipId) 
            : base(Guid.NewGuid())
        {
            ShipId = shipId;
        }
    }
}