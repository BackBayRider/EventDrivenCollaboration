using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;
using ShipRegistryCore.Ports.Queries;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class ShippingLinesAllQueryHandler : QueryHandlerAsync<ShippingLinesAllQuery, ShippingLinesAllQueryResult>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public ShippingLinesAllQueryHandler(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<ShippingLinesAllQueryResult> ExecuteAsync(ShippingLinesAllQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var lines = await uow.Lines.ToArrayAsync(cancellationToken);
                return new ShippingLinesAllQueryResult(lines);
            }
        }
    }
}