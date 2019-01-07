using NUnit.Framework;
using ShipRegistryCore.Ports.Handlers;

namespace Tests
{
    public class ShipRegistrationTests
    {
        [Test]
        public void When_registering_a_new_ship()
        {
            //arrange
            var hander = new NewShipRegistrationHandler();

            //act

            //assert
        }
    }
}