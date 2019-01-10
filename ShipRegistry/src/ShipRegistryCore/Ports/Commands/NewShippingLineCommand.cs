using System;
using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Commands
{
    public class NewShippingLineCommand : Command
    {
        public LineName LineName { get; }

        public NewShippingLineCommand(LineName lineName)
            :base(Guid.NewGuid())
        {
            LineName = lineName;
        }
    }
}