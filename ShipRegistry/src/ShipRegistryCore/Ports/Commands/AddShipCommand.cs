using System;
using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Commands
{
    public class AddShipCommand : Command
    {
        public ShipType Type { get; }
        public ShipName Name { get; }
        public Capacity Capacity { get; }
        public Id ShippingLine { get; }

        public AddShipCommand(ShipType type, ShipName name, Capacity capacity, Id shippingLine) 
            : base(Guid.NewGuid())
        {
            Type = type;
            Name = name;
            Capacity = capacity;
            ShippingLine = shippingLine;
        }
    }
}