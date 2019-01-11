using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Application;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class NewShipRegistrationHandlerAsync : RequestHandlerAsync<AddShipCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public NewShipRegistrationHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }


        public override async Task<AddShipCommand> HandleAsync(AddShipCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var repo = new ShipRepositoryAsync(uow);
                await repo.AddAsync(
                        new Ship(
                            new Id(command.Id), 
                            command.Name,
                            command.Type, 
                            command.Capacity, 
                            command.ShippingLine)
                    );
            }
            
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}