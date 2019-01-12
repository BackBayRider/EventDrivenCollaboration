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

namespace ShipRegitryTests.Ports
{
    [TestFixture]
    public class ShippingLineRegistrationTests
    {
        private DbContextOptions<ShipRegistryDbContext> _options;

        [SetUp]
        public void Setup()
        {
             _options = new DbContextOptionsBuilder<ShipRegistryDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
        }
        
        
        [Test]
        public async Task When_registering_a_new_shipping_line()
        {
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                //arrange
                var commandProcessor = new FakeCommandProcessor();
                
                var newShippingLineCommand = new AddShippingLineCommand(new LineName("Maersk"));
                var handler = new NewShippingLineHandlerAsync(contextFactory, commandProcessor);
                
                //act
                await handler.HandleAsync(newShippingLineCommand);

                //assert
                var uow = contextFactory.Create();
                var line = uow.Lines.SingleOrDefault(l => l.Id == new Id(newShippingLineCommand.Id));
                
                Assert.That(line, Is.Not.Null);
                Assert.That(line.Id, Is.EqualTo(new Id(newShippingLineCommand.Id)));
                Assert.That(line.LineName, Is.EqualTo(newShippingLineCommand.LineName));
                
                var domainEvent = commandProcessor.Messages.SingleOrDefault(m => m.Action == Action.Post);
                Assert.That(domainEvent, Is.Not.Null);

                var addedMessage = (NewLineAddedEvent) domainEvent.Message;
                Assert.That(addedMessage, Is.Not.Null);
                Assert.That(addedMessage.LineId, Is.EqualTo(new Id(newShippingLineCommand.Id)));
                Assert.That(addedMessage.LineName, Is.EqualTo(newShippingLineCommand.LineName));
              }

        }

        [Test]
        public async Task When_getting_a_shipping_line()
        {
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                //arrange
                var uow = contextFactory.Create();
                var repository = new ShippingLineRepositoryAsync(uow);
                var existingLine = new ShippingLine(new Id(), new LineName("Maersk"));
                await repository.AddAsync(existingLine);

                var query = new ShippingLineByIdQuery(existingLine.Id);
                var queryHandler = new ShippingLineByIdQueryHandler(contextFactory);
                
                //act
                var line = await queryHandler.ExecuteAsync(query);
                
                //assert
                Assert.That(line, Is.Not.Null);
                Assert.That(line.Id, Is.EqualTo(existingLine.Id.Value));
                Assert.That(line.LineName, Is.EqualTo(existingLine.LineName.ToString()));
            }
        }

        [Test]
        public async Task When_getting_all_shipping_lines()
        {
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                //arrange
                var uow = contextFactory.Create();
                var repository = new ShippingLineRepositoryAsync(uow);
                
                var existingLineOne = new ShippingLine(new Id(), new LineName("Maersk"));
                await repository.AddAsync(existingLineOne);
                
                var existingLineTwo = new ShippingLine(new Id(), new LineName("Onassis"));
                await repository.AddAsync(existingLineTwo);

                var query = new ShippingLinesAllQuery();
                var queryHandler = new ShippingLinesAllQueryHandler(contextFactory);
                
                //act
                var lines = await queryHandler.ExecuteAsync(query);
                
                //assert
                Assert.That(lines, Is.Not.Null);
                Assert.That(2, Is.EqualTo(lines.ShippingLines.Length));
            }
        }

        [Test]
        public async Task When_updating_a_shipping_line_name()
        {
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                //arrange
                var uow = contextFactory.Create();
                var repository = new ShippingLineRepositoryAsync(uow);
                var existingLine = new ShippingLine(new Id(), new LineName("Maersk"));
                await repository.AddAsync(existingLine);
                
                var commandProcessor = new FakeCommandProcessor();
                
                var command = new UpdateLineNameCommand(existingLine.Id, new LineName("Onassis"));
                
                var handler = new UpdateLineNameHandler(contextFactory, commandProcessor);
                
                //act
                await handler.HandleAsync(command);
                
                //assert
                var line = uow.Lines.SingleOrDefault(l => l.Id == existingLine.Id);
                Assert.That(line.LineName, Is.EqualTo(new LineName("Onassis")));
                Assert.That(line.Version, Is.EqualTo(1));
                
                var domainEvent = commandProcessor.Messages.SingleOrDefault(m => m.Action == Action.Post);
                Assert.That(domainEvent, Is.Not.Null);

                var addedMessage = (LineNameUpdatedEvent) domainEvent.Message;
                Assert.That(addedMessage.LineName, Is.EqualTo(new LineName("Onassis")));
                Assert.That(addedMessage.Version, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task When_removing_a_shipping_line()
        {
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                //arrange
                var uow = contextFactory.Create();
                var repository = new ShippingLineRepositoryAsync(uow);
                var existingLine = new ShippingLine(new Id(), new LineName("Maersk"));
                await repository.AddAsync(existingLine);
                
                var command = new RemoveLineCommand(existingLine.Id);
                
                var handler = new RemoveLineHandlerAsync(contextFactory);
                
                //act
                await handler.HandleAsync(command);
                
                //assert
                var line = uow.Lines.SingleOrDefault(l => l.Id == existingLine.Id);
                Assert.That(line, Is.Null);
            }
        }
            
    }
}