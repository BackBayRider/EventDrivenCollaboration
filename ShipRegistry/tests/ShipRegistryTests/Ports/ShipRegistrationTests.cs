using System;
using System.Linq;
using FreightCaptainTests.Test_Doubles;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ShipRegistryCore.Adapters.Db;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Application;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Handlers;

namespace Tests
{
    public class ShipRegistrationTests
    {
        [Test]
        public void When_registering_a_new_ship()
        {
            //arrange
            var options = new DbContextOptionsBuilder<ShipRegistryDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

            using (var contextFactory = new FakeShipRegistryContextFactory(options))
            {
                var uow = contextFactory.Create();
                
                var lineRepo = new ShippingLineRepositoryAsync(uow);

                var shippingLine = lineRepo.AddAsync(new ShippingLine(new Id(), new LineName("Maersk"))).Result;

                var handler = new NewShipRegistrationHandlerAsync(contextFactory);

                var newShipCommand = new NewShipCommand(
                    type: ShipType.Tanker,
                    name: new ShipName("MV Toronto Star"),
                    capacity: new Capacity(30000),
                    shippingLine: shippingLine.Id);

                //act
                handler.HandleAsync(newShipCommand);

                //assert
                var ship = uow.Ships.SingleOrDefault(s => s.Id == new Id(newShipCommand.Id));
                Assert.That(ship, Is.Not.Null);
                Assert.That(ship.ShipType, Is.EqualTo(newShipCommand.Type));
                Assert.That(ship.ShipName, Is.EqualTo(newShipCommand.Name));
                Assert.That(ship.Capacity, Is.EqualTo(newShipCommand.Capacity));
                Assert.That(ship.Id, Is.EqualTo(new Id(newShipCommand.Id)));
                Assert.That(ship.ShippingLineId, Is.EqualTo(newShipCommand.ShippingLine));
             }
        }
    }
}