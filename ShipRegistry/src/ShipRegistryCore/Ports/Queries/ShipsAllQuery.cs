using System.Collections.Generic;
using System.Linq;
using Paramore.Darker;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Queries
{
    public class ShipsAllQuery : IQuery<ShipsAllQueryResult>{}
    
    public class ShipsAllQueryResult
    {
        public ShipByIdQueryResult[] Ships { get; }

        public ShipsAllQueryResult(IEnumerable<Ship> ships)
        {
            Ships = ships.Select(s => new ShipByIdQueryResult(s)).ToArray();
        }
    }
}