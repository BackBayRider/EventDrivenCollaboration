using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Exceptions;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class UpdateShipOwnerCommandHandlerAsync : RequestHandlerAsync<UpdateShipOwnerCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public UpdateShipOwnerCommandHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<UpdateShipOwnerCommand> HandleAsync(UpdateShipOwnerCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var repository = new ShipRepositoryAsync(uow);
                var ship = await repository.GetAsync(command.ShipId, cancellationToken);
                if (ship == null) throw new NotFoundException($"Could not find a ship with Id {command.ShipId}");

                ship.ShippingLineId = command.ShippingLineId;

                await repository.UpdateAsync(ship, cancellationToken);

            }
                        
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}