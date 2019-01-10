using System;
using Microsoft.EntityFrameworkCore;
using ShipRegistryCore.Adapters.Db;
using ShipRegistryCore.Ports.Repositories;

namespace FreightCaptainTests.Test_Doubles
{
    public class FakeShipRegistryContextFactory : IShipRegistryContextFactory, IDisposable
    {
        private ShipRegistryDbContext _context;

        public FakeShipRegistryContextFactory(DbContextOptions<ShipRegistryDbContext> options)
        {
            //in the fake we want to re-use the UoW to allow us to populate for tests
            _context = new FakeShipRegistryDbContext(options);
        }

        public ShipRegistryDbContext Create()
        {
            return _context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}