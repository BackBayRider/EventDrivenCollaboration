using System;
using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Commands
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