using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryPorts.Commands;
using ShipRegistryPorts.Events;
using ShipRegistryPorts.Exceptions;
using ShipRegistryPorts.Repositories;

namespace ShipRegistryPorts.Handlers
{
    public class UpdateLineNameHandler : RequestHandlerAsync<UpdateLineNameCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;
        private readonly IAmACommandProcessor _commandProcessor;

        public UpdateLineNameHandler(IShipRegistryContextFactory contextFactory, IAmACommandProcessor commandProcessor)
        {
            _contextFactory = contextFactory;
            _commandProcessor = commandProcessor;
        }

        public override async Task<UpdateLineNameCommand> HandleAsync(UpdateLineNameCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var repository = new ShippingLineRepositoryAsync(uow);
                var line = await repository.GetAsync(command.LineId);
                if (line == null) throw new NotFoundException($"Could not find a line with the Id: {command.LineId}");

                line.LineName = command.LineName;

                await repository.UpdateAsync(line);

                await _commandProcessor.PostAsync(new LineNameUpdatedEvent(line.Id, line.LineName, line.Version));

            }
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}