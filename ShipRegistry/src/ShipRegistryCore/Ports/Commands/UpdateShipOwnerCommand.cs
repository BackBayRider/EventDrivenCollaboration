using System;
using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Commands
{
    public class UpdateShipOwnerCommand: Command
    {
        public Id ShipId { get; }
        public Id ShippingLineId { get; }

        public UpdateShipOwnerCommand(Id shipId, Id shippingLineId) 
            : base(Guid.NewGuid())
        {
            ShipId = shipId;
            ShippingLineId = shippingLineId;
        }
    }
}