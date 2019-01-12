using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Exceptions;
using ShipRegistryCore.Ports.Repositories;

namespace ShipRegistryCore.Ports.Handlers
{
    public class UpdateLineNameHandler : RequestHandlerAsync<UpdateLineNameCommand>
    {
        private readonly IShipRegistryContextFactory _contextFactory;

        public UpdateLineNameHandler(IShipRegistryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
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

            }
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}