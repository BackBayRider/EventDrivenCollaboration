using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Events;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
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