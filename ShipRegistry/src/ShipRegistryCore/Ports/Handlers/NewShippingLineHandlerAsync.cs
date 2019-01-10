using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Application;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class NewShippingLineHandlerAsync : RequestHandlerAsync<NewShippingLineCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public NewShippingLineHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }


        public override async Task<NewShippingLineCommand> HandleAsync(NewShippingLineCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var repository = new ShippingLineRepositoryAsync(uow);
                await repository.AddAsync(
                    new ShippingLine(
                        new Id(command.Id),
                        command.LineName)
                );
            }
           
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}