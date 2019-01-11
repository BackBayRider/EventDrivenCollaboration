using System;
using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Commands
{
    public class AddShippingLineCommand : Command
    {
        public LineName LineName { get; }

        public AddShippingLineCommand(LineName lineName)
            :base(Guid.NewGuid())
        {
            LineName = lineName;
        }
    }
}