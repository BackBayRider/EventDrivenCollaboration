using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;
using ShipRegistryPorts.Commands;
using ShipRegistryPorts.Events;
using ShipRegistryPorts.Exceptions;
using ShipRegistryPorts.Repositories;

namespace ShipRegistryPorts.Handlers
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

        [RequestLoggingAsync(step: 1, timing: HandlerTiming.Before)]
        [UsePolicyAsync(policy: CommandProcessor.CIRCUITBREAKERASYNC, step:2)]
        [UsePolicyAsync(policy: CommandProcessor.RETRYPOLICYASYNC, step: 3)]
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