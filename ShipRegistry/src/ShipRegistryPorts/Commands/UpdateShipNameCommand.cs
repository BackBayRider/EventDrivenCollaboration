using System;
using Paramore.Brighter;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Commands
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