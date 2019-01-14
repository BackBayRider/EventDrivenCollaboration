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
    public class ShipsAllQueryHandlerAsync : QueryHandlerAsync<ShipsAllQuery, ShipsAllQueryResult>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public ShipsAllQueryHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
        [QueryLogging(1)]
        [RetryableQuery(2)]
        public override async Task<ShipsAllQueryResult> ExecuteAsync(ShipsAllQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var ships = await uow.Ships.ToArrayAsync(cancellationToken);

                return new ShipsAllQueryResult(ships);
            }
 
        }
    }
}