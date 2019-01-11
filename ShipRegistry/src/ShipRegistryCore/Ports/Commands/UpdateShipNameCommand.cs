using System;
using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Commands
{
    public class UpdateShipNameCommand : Command
    {
        public Id ShipId { get; }
        public ShipName Name { get; }

        public UpdateShipNameCommand(Id shipId, ShipName name) 
            : base(Guid.NewGuid())
        {
            ShipId = shipId;
            Name = name;
        }
 
    }
}