using System;

namespace ShipRegistryCore.Application
{
    public class ShippingLine : IEntity
    {
        public Id Id { get; set; }
        public LineName LineName { get; set; }
        public int Version { get; set; }

        public ShippingLine(Id id, LineName lineName)
        {
            Id = id;
            LineName = lineName;
            Version = 0;
        }

   }
}