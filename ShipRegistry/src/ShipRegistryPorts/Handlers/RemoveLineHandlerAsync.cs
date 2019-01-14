using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;
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

        [RequestLoggingAsync(step: 1, timing: HandlerTiming.Before)]
        [UsePolicyAsync(policy: CommandProcessor.CIRCUITBREAKERASYNC, step:2)]
        [UsePolicyAsync(policy: CommandProcessor.RETRYPOLICYASYNC, step: 3)]
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