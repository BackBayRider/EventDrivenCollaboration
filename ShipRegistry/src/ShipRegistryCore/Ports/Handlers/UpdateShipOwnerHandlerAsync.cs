using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Events;
using ShipRegistryCore.Ports.Exceptions;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class UpdateShipOwnerHandlerAsync : RequestHandlerAsync<UpdateShipOwnerCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;
        private readonly IAmACommandProcessor _commandProcessor;

        public UpdateShipOwnerHandlerAsync(
            IShipRegistryContextFactory contextFactory,
            IAmACommandProcessor commandProcessor)
        {
            _contextFactory = contextFactory;
            _commandProcessor = commandProcessor;
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

                await _commandProcessor.PostAsync(new ShipOwnerUpdatedEvent(ship.Id, command.ShippingLineId, ship.Version));
            }
                        
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}