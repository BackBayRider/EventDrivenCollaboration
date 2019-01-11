using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;
using ShipRegistryCore.Ports.Queries;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class ShipsAllQueryHandlerAsync : QueryHandlerAsync<ShipsAllQuery, ShipsAllQueryResult>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public ShipsAllQueryHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
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