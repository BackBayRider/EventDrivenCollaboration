using ShipRegistryPorts.Db;

namespace ShipRegistryPorts.Repositories
{
    public interface IShipRegistryContextFactory
    {
        ShipRegistryDbContext Create();
    }
}