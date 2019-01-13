using Newtonsoft.Json;
using Paramore.Brighter;
using ShipRegistryPorts.Events;

namespace ShipRegistryPorts.Mappers
{
    public class ShipNameUpdatedEventMessageMapper : IAmAMessageMapper<ShipNameUpdatedEvent>
    {
        public Message MapToMessage(ShipNameUpdatedEvent request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "ShipRegistration.Ship.ShipNameUpdated.Event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public ShipNameUpdatedEvent MapToRequest(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}