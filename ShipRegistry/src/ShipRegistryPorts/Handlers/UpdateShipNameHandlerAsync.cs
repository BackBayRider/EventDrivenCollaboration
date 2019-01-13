using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryPorts.Commands;
using ShipRegistryPorts.Events;
using ShipRegistryPorts.Exceptions;
using ShipRegistryPorts.Repositories;

namespace ShipRegistryPorts.Handlers
{
    public class UpdateShipNameHandlerAsync : RequestHandlerAsync<UpdateShipNameCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;
        private readonly IAmACommandProcessor _commandProcessor;

        public UpdateShipNameHandlerAsync(
            IShipRegistryContextFactory contextFactory,
            IAmACommandProcessor commandProcessor)
        {
            _contextFactory = contextFactory;
            _commandProcessor = commandProcessor;
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

                await _commandProcessor.PostAsync(new ShipNameUpdatedEvent(ship.Id, command.Name, ship.Version));
            }
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}