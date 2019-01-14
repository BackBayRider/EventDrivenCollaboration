using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;
using ShipRegistryApplication;
using ShipRegistryAPI.ViewModels;
using ShipRegistryPorts.Commands;
using ShipRegistryPorts.Queries;

namespace ShipRegistryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipRegistryController : ControllerBase
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public ShipRegistryController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ships = await _queryProcessor.ExecuteAsync(new ShipsAllQuery());
            return Ok(ships.Ships);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var greeting = await _queryProcessor.ExecuteAsync(new ShipByIdQuery(new Id(id)));
            return Ok(greeting);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddShipRequest request)
        {
            var addShipCommand = new AddShipCommand(
                type: (ShipType)Enum.Parse<ShipType>(request.Type),
                name: new ShipName(request.Name),
                capacity: new Capacity(request.Capacity),
                shippingLine:new Id(request.ShippingLine)
                );

            await _commandProcessor.SendAsync(addShipCommand);

            var addedShip = await _queryProcessor.ExecuteAsync(new ShipByIdQuery(new Id(addShipCommand.Id)));
            
            return Ok(addedShip);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] PatchOperation[] shipPatchOperations)
        {
            var updateNameOps =
                shipPatchOperations
                    .Where(o => o.Op == "replace")
                    .Where(o => o.Path.Contains("Name"))
                    .Select(o => new UpdateShipNameCommand(new Id(id), new ShipName(o.Value)));
            
            var updateOwnerOps =
                shipPatchOperations
                    .Where(o => o.Op == "replace")
                    .Where(o => o.Path.Contains("Owner"))
                    .Select(o => new UpdateShipOwnerCommand(new Id(id), new Id(Guid.Parse(o.Value))));

            foreach (var updateName in updateNameOps)
            {
                await _commandProcessor.SendAsync(updateName);
            }

            foreach (var updateOwner in updateOwnerOps)
            {
                await _commandProcessor.SendAsync(updateOwner);
            }
            
            var updatedShip = await _queryProcessor.ExecuteAsync(new ShipByIdQuery(new Id(id)));
            
            return Ok(updatedShip);
         }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleteGreetingCommand = new RemoveShipCommand(new Id(id));
            await _commandProcessor.SendAsync(deleteGreetingCommand);
            return Ok();
        }
 
    }
}