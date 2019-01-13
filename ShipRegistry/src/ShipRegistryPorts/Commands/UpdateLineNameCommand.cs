using System;
using Paramore.Brighter;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Commands
{
    public class UpdateLineNameCommand : Command
    {
        public Id LineId { get; }
        public LineName LineName { get; }

        public UpdateLineNameCommand(Id lineId, LineName lineName) 
            : base(Guid.NewGuid())
        {
            LineId = lineId;
            LineName = lineName;
        }
    }
}