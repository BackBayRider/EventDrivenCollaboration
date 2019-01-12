using System;
using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Commands
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