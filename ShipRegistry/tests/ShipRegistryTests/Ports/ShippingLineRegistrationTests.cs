using System.Linq;
using System.Threading.Tasks;
using FreightCaptainTests.Test_Doubles;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ShipRegistryCore.Adapters.Db;
using ShipRegistryCore.Application;
using ShipRegistryCore.Ports.Commands;
using ShipRegistryCore.Ports.Handlers;

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
                var newShippingLineCommand = new AddShippingLineCommand(new LineName("Maersk"));
                var handler = new NewShippingLineHandlerAsync(contextFactory);
                
                //act
                await handler.HandleAsync(newShippingLineCommand);

                //assert
                var uow = contextFactory.Create();
                var line = uow.Lines.SingleOrDefault(l => l.Id == new Id(newShippingLineCommand.Id));
                
                Assert.That(line, Is.Not.Null);
                Assert.That(line.Id, Is.EqualTo(new Id(newShippingLineCommand.Id)));
                Assert.That(line.LineName, Is.EqualTo(newShippingLineCommand.LineName));
            }

        }

        [Test]
        public async Task When_getting_a_shipping_line()
        {
            using (var contextFactory = new FakeShipRegistryContextFactory(_options))
            {
                var query     
            }
        }
    }
    
}