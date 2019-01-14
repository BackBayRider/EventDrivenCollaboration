using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;
using ShipRegistryPorts.Commands;
using ShipRegistryPorts.Events;
using ShipRegistryPorts.Repositories;

namespace ShipRegistryPorts.Handlers
{
    public class RemoveShipHandlerAsync : RequestHandlerAsync<RemoveShipCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;
        private readonly IAmACommandProcessor _commandProcessor;

        public RemoveShipHandlerAsync(
            IShipRegistryContextFactory contextFactory, 
            IAmACommandProcessor commandProcessor)
        {
            _contextFactory = contextFactory;
            _commandProcessor = commandProcessor;
        }


        [RequestLoggingAsync(step: 1, timing: HandlerTiming.Before)]
        [UsePolicyAsync(policy: CommandProcessor.CIRCUITBREAKERASYNC, step:2)]
        [UsePolicyAsync(policy: CommandProcessor.RETRYPOLICYASYNC, step: 3)]
 public override async Task<RemoveShipCommand> HandleAsync(RemoveShipCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {

                var repository = new ShipRepositoryAsync(uow);
                await repository.DeleteAsync(command.ShipId, cancellationToken);

                await _commandProcessor.PostAsync(new ShipRemovedEvent(command.ShipId));

            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}