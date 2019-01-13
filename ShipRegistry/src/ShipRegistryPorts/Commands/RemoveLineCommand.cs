using System;
using Paramore.Brighter;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Commands
{
    public class RemoveLineCommand : Command
    {
        public Id LineId { get; }

        public RemoveLineCommand(Id lineId) 
            : base(Guid.NewGuid())
        {
            LineId = lineId;
        }
    }
}