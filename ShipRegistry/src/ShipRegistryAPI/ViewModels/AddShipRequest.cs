using System;

namespace ShipRegistryAPI.ViewModels
{
    public class AddShipRequest
    {
        public string Type { get; }
        public string Name { get; }
        public int Capacity { get; }
        public Guid ShippingLine { get; }

    }
}