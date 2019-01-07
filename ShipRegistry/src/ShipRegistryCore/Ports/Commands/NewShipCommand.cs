using System;
using Paramore.Brighter;

namespace ShipRegistryCore.Ports.Commands
{
    public class NewShipCommand : Command
    {
        public NewShipCommand(Guid id) : base(id)
        {
        }
    }
}