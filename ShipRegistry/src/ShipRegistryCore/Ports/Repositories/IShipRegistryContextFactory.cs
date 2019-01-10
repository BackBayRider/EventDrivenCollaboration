using System;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using ShipRegistryCore.Adapters.Db;

namespace ShipRegistryCore.Ports.Repositories
{
    public interface IShipRegistryContextFactory
    {
        ShipRegistryDbContext Create();
    }
}