using Microsoft.EntityFrameworkCore;
using ShipRegistryPorts.Db;
using ShipRegistryPorts.Repositories;

namespace FreightCaptainTests.Test_Doubles
{
    //We want this wrapper so that we can avoid disposing the DbContext before we have interrogated it in a test
    public class FakeShipRegistryDbContext : ShipRegistryDbContext
    {
        public FakeShipRegistryDbContext(DbContextOptions<ShipRegistryDbContext> options) 
            : base(options)
        {}

        public override void Dispose()
        {
        }
    }
}