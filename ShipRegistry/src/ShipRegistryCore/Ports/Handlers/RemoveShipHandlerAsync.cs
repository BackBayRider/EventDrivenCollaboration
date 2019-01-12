using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class RemoveShipHandlerAsync : RequestHandlerAsync<RemoveShipCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public RemoveShipHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }


        public override async Task<RemoveShipCommand> HandleAsync(RemoveShipCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {

                var repository = new ShipRepositoryAsync(uow);
                await repository.DeleteAsync(command.ShipId, cancellationToken);

            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}