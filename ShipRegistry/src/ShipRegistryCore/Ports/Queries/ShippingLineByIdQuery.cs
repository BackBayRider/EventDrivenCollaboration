using System;
using Paramore.Darker;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Queries
{
    public class ShippingLineByIdQuery : IQuery<ShippingLineByIdQueryResult>
    {
        public Id LineId { get; }

        public ShippingLineByIdQuery(Id lineId)
        {
            LineId = lineId;
        }
    }

    public class ShippingLineByIdQueryResult
    {
        public Guid Id { get; }
        public string LineName { get; }

        public ShippingLineByIdQueryResult(ShippingLine shippingLine)
        {
            Id = shippingLine.Id.Value;
            LineName = shippingLine.LineName.ToString();
        }
    }
}