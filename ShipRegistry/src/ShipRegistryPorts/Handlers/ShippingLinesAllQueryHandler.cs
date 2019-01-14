using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;
using Paramore.Darker.Policies;
using Paramore.Darker.QueryLogging;
using ShipRegistryPorts.Queries;
using ShipRegistryPorts.Repositories;

namespace ShipRegistryPorts.Handlers
{
    public class ShippingLinesAllQueryHandler : QueryHandlerAsync<ShippingLinesAllQuery, ShippingLinesAllQueryResult>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public ShippingLinesAllQueryHandler(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [QueryLogging(1)]
        [RetryableQuery(2)]
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