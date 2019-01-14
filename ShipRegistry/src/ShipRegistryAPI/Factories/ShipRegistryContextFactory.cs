using Microsoft.EntityFrameworkCore;
using ShipRegistryPorts.Db;
using ShipRegistryPorts.Repositories;

namespace ShipRegistryAPI.Factories
{
    public class ShipRegistryContextFactory : IShipRegistryContextFactory
    {
        private readonly DbContextOptions<ShipRegistryDbContext> _options;

        public ShipRegistryContextFactory(DbContextOptions<ShipRegistryDbContext> options)
        {
            _options = options;
        }
        
        public ShipRegistryDbContext Create()
        {
           return new ShipRegistryDbContext(_options); 
        }
    }
}