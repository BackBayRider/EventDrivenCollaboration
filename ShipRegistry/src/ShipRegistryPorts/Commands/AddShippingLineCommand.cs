using System;
using Paramore.Brighter;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Commands
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