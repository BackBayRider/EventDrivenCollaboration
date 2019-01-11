using System;
using System.Linq;
using System.Threading.Tasks;
using FreightCaptainTests.Test_Doubles;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ShipRegistryCore.Adapters.Db;
using ShipRegistryCore.Adapters.Repositories;
using ShipRegistryCore.Application;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Handlers;
using ShipRegistryCore.Ports.Queries;

namespace ShipRegitryTests.Ports
{
    [TestFixture]
    public class ShipRegistrationTests
    {
        private DbContextOptions<ShipRegistryDbContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<ShipRegistryDbContext>().UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
        }
        
        
        [Test]
        public async Task When_registering_a_new_ship()
        {
            //arrange
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                var uow = contextFactory.Create();
                
                var lineRepo = new ShippingLineRepositoryAsync(uow);

                var shippingLine = lineRepo.AddAsync(new ShippingLine(new Id(), new LineName("Maersk"))).Result;

                var handler = new NewShipRegistrationHandlerAsync(contextFactory);

                var newShipCommand = new AddShipCommand(
                    type: ShipType.Tanker,
                    name: new ShipName("MV Toronto Star"),
                    capacity: new Capacity(30000),
                    shippingLine: shippingLine.Id);

                //act
                await handler.HandleAsync(newShipCommand);

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

        [Test]
        public async Task When_getting_a_ship()
        {
            //arrange
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                var uow = contextFactory.Create();
                var repository = new ShipRepositoryAsync(uow);
                var ship = new Ship(new Id(), new ShipName("Majestic"), ShipType.Container, new Capacity(50000), new Id() );

                await repository.AddAsync(ship);

                var query = new ShipByIdQuery(ship.Id);
                
                var queryHandler = new ShipByIdQueryHandlerAsync(contextFactory);

                //act
                var foundShip = await queryHandler.ExecuteAsync(query);

                //asert
                Assert.That(foundShip.Id, Is.EqualTo(ship.Id.Value));
                Assert.That(foundShip.Capacity, Is.EqualTo(ship.Capacity.Value));
                Assert.That(foundShip.ShipName, Is.EqualTo(ship.ShipName.ToString()));
                Assert.That(foundShip.ShipType, Is.EqualTo(ship.ShipType.ToString()));
                Assert.That(foundShip.ShippingLineId, Is.EqualTo(ship.ShippingLineId.Value));
            }
        }

        [Test]
        public async Task When_getting_all_ships()
        {
            //arrange
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                var uow = contextFactory.Create();
                var repository = new ShipRepositoryAsync(uow);
                var shipOne = new Ship(new Id(), new ShipName("Majestic"), ShipType.Container, new Capacity(50000), new Id() );
                var shipTwo = new Ship(new Id(), new ShipName("Royal"), ShipType.Container, new Capacity(80000), new Id() );

                await repository.AddAsync(shipOne);
                await repository.AddAsync(shipTwo);

                var query = new ShipsAllQuery();
                
                var queryHandler = new ShipsAllQueryHandlerAsync(contextFactory);

                //act
                var result = await queryHandler.ExecuteAsync(query);

                //asert
                Assert.That(2, Is.EqualTo(result.Ships));
           }
             
        }

        [Test]
        public async Task When_updating_an_existing_ship_name()
        {
            //arrange
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                var uow = contextFactory.Create();
                var repository = new ShipRepositoryAsync(uow);
                var ship = new Ship(new Id(), new ShipName("Majestic"), ShipType.Container, new Capacity(50000), new Id() );

                await repository.AddAsync(ship);

                var command = new UpdateShipNameCommand(ship.Id, name: new ShipName("Toronto Star"));

                var handler = new UpdateShipNameCommandHandlerAsync(contextFactory);

                //act
                await handler.HandleAsync(command);

                //assert
                var updatedShip = await uow.Ships.SingleOrDefaultAsync(s => s.Id == ship.Id);
                Assert.That(updatedShip , Is.Not.Null);
                Assert.That(updatedShip.ShipName, Is.EqualTo(command.Name));
            }
        }

        [Test]
        public async Task When_updating_an_existing_ships_owner()
        {
            //arrange
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                var uow = contextFactory.Create();
                var repository = new ShipRepositoryAsync(uow);
                var ship = new Ship(new Id(), new ShipName("Majestic"), ShipType.Container, new Capacity(50000), new Id() );

                await repository.AddAsync(ship);

                var command = new UpdateShipOwnerCommand(ship.Id, new Id(Guid.NewGuid())); 

                var handler = new UpdateShipOwnerCommandHandlerAsync(contextFactory); 

                //act
                await handler.HandleAsync(command);

                //assert
                var updatedShip = await uow.Ships.SingleOrDefaultAsync(s => s.Id == ship.Id);
                Assert.That(updatedShip , Is.Not.Null);
                Assert.That(updatedShip.ShippingLineId, Is.EqualTo(command.ShippingLineId));
            }
        }
    }
}