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
    public class LineRegistryController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public LineRegistryController(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lines = await _queryProcessor.ExecuteAsync<ShippingLinesAllQueryResult>(new ShippingLinesAllQuery());
            return Ok(lines.ShippingLines);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var line = await _queryProcessor.ExecuteAsync(new ShippingLineByIdQuery(new Id(id)));
            return Ok( line);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddShippingLineRequest request)
        {
            var addShippingLineCommand = new AddShippingLineCommand(
                lineName:new LineName(request.LineName)
                );

            await _commandProcessor.SendAsync(addShippingLineCommand);

            var addedShip = await _queryProcessor.ExecuteAsync(new ShippingLineByIdQuery(new Id(addShippingLineCommand.Id)));
            
            return Ok(addedShip);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] PatchOperation[] linePatchOperations)
        {
            var updateNameOps =
                linePatchOperations
                    .Where(o => o.Op == "replace")
                    .Where(o => o.Path.Contains("Name"))
                    .Select(o => new UpdateLineNameCommand(new Id(id), new LineName(o.Value)));
            

            foreach (var updateName in updateNameOps)
            {
                await _commandProcessor.SendAsync(updateName);
            }

            var updatedLine = await _queryProcessor.ExecuteAsync(new ShippingLineByIdQuery(new Id(id)));
            
            return Ok(updatedLine);
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