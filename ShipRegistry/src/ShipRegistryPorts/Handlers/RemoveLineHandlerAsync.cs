using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryPorts.Commands;
using ShipRegistryPorts.Repositories;

namespace ShipRegistryPorts.Handlers
{
    public class RemoveLineHandlerAsync : RequestHandlerAsync<RemoveLineCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public RemoveLineHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<RemoveLineCommand> HandleAsync(RemoveLineCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var repo = new ShippingLineRepositoryAsync(uow);
                await repo.DeleteAsync(command.LineId);
            }
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}