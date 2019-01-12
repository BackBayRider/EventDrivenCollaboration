using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Application;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Events;
using ShipRegistryCore.Ports.Queries;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class NewShipRegistrationHandlerAsync : RequestHandlerAsync<AddShipCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;
        private readonly IAmACommandProcessor _commandProcessor;

        public NewShipRegistrationHandlerAsync(
            IShipRegistryContextFactory contextFactory,
            IAmACommandProcessor commandProcessor)
        {
            _contextFactory = contextFactory;
            _commandProcessor = commandProcessor;
        }


        public override async Task<AddShipCommand> HandleAsync(AddShipCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var repo = new ShipRepositoryAsync(uow);
                var ship = new Ship(new Id(command.Id), command.Name,command.Type, command.Capacity, command.ShippingLine);
                
                await repo.AddAsync(ship);
                
                await _commandProcessor.PostAsync(
                    new NewShipAddedEvent(ship.Id, ship.ShipType, ship.ShipName, ship.Capacity, ship.ShippingLineId),
                    cancellationToken: cancellationToken
                    );
            }
           
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}