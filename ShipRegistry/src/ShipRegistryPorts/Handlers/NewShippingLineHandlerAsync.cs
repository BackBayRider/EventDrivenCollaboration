using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryApplication;
using ShipRegistryPorts.Commands;
using ShipRegistryPorts.Events;
using ShipRegistryPorts.Repositories;

namespace ShipRegistryPorts.Handlers
{
    public class NewShippingLineHandlerAsync : RequestHandlerAsync<AddShippingLineCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;
        private readonly IAmACommandProcessor _commandProcessor;

        public NewShippingLineHandlerAsync(
            IShipRegistryContextFactory contextFactory,
            IAmACommandProcessor commandProcessor)
        {
            _contextFactory = contextFactory;
            _commandProcessor = commandProcessor;
        }


        public override async Task<AddShippingLineCommand> HandleAsync(AddShippingLineCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var repository = new ShippingLineRepositoryAsync(uow);
                var line = new ShippingLine(new Id(command.Id),command.LineName);
                await repository.AddAsync(line);

                await _commandProcessor.PostAsync(new NewLineAddedEvent(line.Id, line.LineName));
            }
           
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}