using System;
using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Commands
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