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
using ShipRegistryCore.Ports.Events;
using ShipRegistryCore.Ports.Handlers;
using ShipRegistryCore.Ports.Queries;
using Action = FreightCaptainTests.Test_Doubles.Action;

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
                
                var commandProcessor = new FakeCommandProcessor();

                var handler = new NewShipRegistrationHandlerAsync(contextFactory, commandProcessor);

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
                Assert.That(ship.Version, Is.EqualTo(0));

                var domainEvent = commandProcessor.Messages.SingleOrDefault(m => m.Action == Action.Post);
                Assert.That(domainEvent, Is.Not.Null);

                var addedMessage = (NewShipAddedEvent) domainEvent.Message;
                Assert.That(addedMessage.Id, Is.EqualTo(newShipCommand.Id));
                Assert.That(addedMessage.Capacity, Is.EqualTo(newShipCommand.Capacity));
                Assert.That(addedMessage.Name, Is.EqualTo(newShipCommand.Name));
                Assert.That(addedMessage.Type, Is.EqualTo(newShipCommand.Type));
                Assert.That(addedMessage.ShipId, Is.EqualTo(new Id(newShipCommand.Id)));
                Assert.That(addedMessage.ShippingLine, Is.EqualTo(newShipCommand.ShippingLine));
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
                Assert.That(2, Is.EqualTo(result.Ships.Length));
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

                var commandProcessor = new FakeCommandProcessor();

                var command = new UpdateShipNameCommand(ship.Id, name: new ShipName("Toronto Star"));

                var handler = new UpdateShipNameHandlerAsync(contextFactory, commandProcessor);

                //act
                await handler.HandleAsync(command);

                //assert
                var updatedShip = await uow.Ships.SingleOrDefaultAsync(s => s.Id == ship.Id);
                Assert.That(updatedShip , Is.Not.Null);
                Assert.That(updatedShip.ShipName, Is.EqualTo(command.Name));
                Assert.That(updatedShip.Version, Is.EqualTo(1));
                Assert.That(ship.Version, Is.EqualTo(1));
       
                var domainEvent = commandProcessor.Messages.SingleOrDefault(m => m.Action == Action.Post);
                Assert.That(domainEvent, Is.Not.Null);

                var updatedNameMessage = (ShipNameUpdatedEvent) domainEvent.Message;
                Assert.That(updatedNameMessage.ShipId, Is.EqualTo(updatedShip.Id));
                Assert.That(updatedNameMessage.ShipName, Is.EqualTo(updatedShip.ShipName));
                
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

                var commandProcessor = new FakeCommandProcessor();

                var command = new UpdateShipOwnerCommand(ship.Id, new Id(Guid.NewGuid())); 

                var handler = new UpdateShipOwnerHandlerAsync(contextFactory, commandProcessor); 

                //act
                await handler.HandleAsync(command);

                //assert
                var updatedShip = await uow.Ships.SingleOrDefaultAsync(s => s.Id == ship.Id);
                Assert.That(updatedShip , Is.Not.Null);
                Assert.That(updatedShip.ShippingLineId, Is.EqualTo(command.ShippingLineId));
                Assert.That(ship.Version, Is.EqualTo(1));
                
                var domainEvent = commandProcessor.Messages.SingleOrDefault(m => m.Action == Action.Post);
                Assert.That(domainEvent, Is.Not.Null);
                
                var shipOwnerUpdatedEvent = (ShipOwnerUpdatedEvent) domainEvent.Message;
                Assert.That(shipOwnerUpdatedEvent.ShipId, Is.EqualTo(updatedShip.Id));
                Assert.That(shipOwnerUpdatedEvent.ShippingLine, Is.EqualTo(updatedShip.ShippingLineId));
             }
        }

        [Test]
        public async Task When_removing_a_ship_registration()
        {
             //arrange
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                var uow = contextFactory.Create();
                var repository = new ShipRepositoryAsync(uow);
                var ship = new Ship(new Id(), new ShipName("Majestic"), ShipType.Container, new Capacity(50000), new Id() );

                await repository.AddAsync(ship);

                var commandProcessor = new FakeCommandProcessor();
                
                var command = new RemoveShipCommand(ship.Id); 

                var handler = new RemoveShipHandlerAsync(contextFactory, commandProcessor); 

                //act
                await handler.HandleAsync(command);

                //assert
                var removedShip = await uow.Ships.SingleOrDefaultAsync(s => s.Id == ship.Id);
                Assert.That(removedShip, Is.Null);
                
                var domainEvent = commandProcessor.Messages.SingleOrDefault(m => m.Action == Action.Post);
                Assert.That(domainEvent, Is.Not.Null);
                
                var shipRemovedEvent = (ShipRemovedEvent) domainEvent.Message;
                Assert.That(shipRemovedEvent.ShipRemoved, Is.EqualTo(ship.Id));
             }
            
        }
  }
}