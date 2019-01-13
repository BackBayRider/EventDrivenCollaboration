using System;
using Paramore.Darker;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Queries
{
    public class ShipByIdQuery : IQuery<ShipByIdQueryResult>
    {
        public Id ShipId { get; }

        public ShipByIdQuery(Id shipId)
        {
            ShipId = shipId;
        }
    }


    public class ShipByIdQueryResult
    {
        public Guid Id { get;}
        public string ShipName { get;}
        public string ShipType { get;}
        public int Capacity { get;}
        public Guid ShippingLineId { get;}
        
        public ShipByIdQueryResult(Ship ship)
        {
            Id = ship.Id.Value;
            ShipName = ship.ShipName.ToString();
            ShipType = ship.ShipType.ToString();
            Capacity = ship.Capacity.Value;
            ShippingLineId = ship.ShippingLineId.Value;
        }
    }
}