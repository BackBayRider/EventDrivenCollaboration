using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class RemoveLineHandlerAsync : RequestHandlerAsync<RemoveLineCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public RemoveLineHandlerAsync(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override Task<RemoveLineCommand> HandleAsync(RemoveLineCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = _contextFactory.Create())
            {
                var repo = new ShippingLineRepositoryAsync(uow);
                repo.DeleteAsync(command.LineId);
            }
            return base.HandleAsync(command, cancellationToken);
        }
    }
}