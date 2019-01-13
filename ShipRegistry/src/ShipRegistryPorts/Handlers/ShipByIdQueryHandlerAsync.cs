using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;
using ShipRegistryPorts.Exceptions;
using ShipRegistryPorts.Queries;
using ShipRegistryPorts.Repositories;

namespace ShipRegistryPorts.Handlers
{
    public class ShipByIdQueryHandlerAsync : QueryHandlerAsync<ShipByIdQuery, ShipByIdQueryResult>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public ShipByIdQueryHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
        
        public override async Task<ShipByIdQueryResult> ExecuteAsync(ShipByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var ship = await uow.Ships.SingleOrDefaultAsync(sh => sh.Id == query.ShipId, cancellationToken);

                if (ship == null) throw new NotFoundException($"Ship with Id {query.ShipId.ToString()} could not be found");

                return new ShipByIdQueryResult(ship);
            }
        }
    }
}