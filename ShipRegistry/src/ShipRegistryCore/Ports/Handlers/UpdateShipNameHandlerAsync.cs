using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Exceptions;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class UpdateShipNameHandlerAsync : RequestHandlerAsync<UpdateShipNameCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public UpdateShipNameHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<UpdateShipNameCommand> HandleAsync(UpdateShipNameCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var repository = new ShipRepositoryAsync(uow);
                var ship = await repository.GetAsync(command.ShipId, cancellationToken);
                if (ship == null) throw new NotFoundException($"Could not find a ship with Id {command.ShipId}");

                ship.ShipName = command.Name;

                await repository.UpdateAsync(ship, cancellationToken);

            }
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}