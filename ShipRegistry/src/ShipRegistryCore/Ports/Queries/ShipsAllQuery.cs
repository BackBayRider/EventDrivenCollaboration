using System.Collections.Generic;
using System.Linq;
using Paramore.Darker;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Queries
{
    public class ShipsAllQuery : IQuery<ShipsAllQueryResult>{}
    
    public class ShipsAllQueryResult
    {
        public ShipByIdQueryResult[] _ships { get; }

        public ShipsAllQueryResult(IEnumerable<Ship> ships)
        {
            _ships = ships.Select(s => new ShipByIdQueryResult(s)).ToArray();
        }
    }
}