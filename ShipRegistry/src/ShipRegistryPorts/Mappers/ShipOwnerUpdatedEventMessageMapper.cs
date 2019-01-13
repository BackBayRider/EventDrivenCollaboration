using Newtonsoft.Json;
using Paramore.Brighter;
using ShipRegistryPorts.Events;

namespace ShipRegistryPorts.Mappers
{
    public class ShipOwnerUpdatedEventMessageMapper : IAmAMessageMapper<ShipOwnerUpdatedEvent>
    {
        public Message MapToMessage(ShipOwnerUpdatedEvent request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "ShipRegistration.Ship.ShipOwnerUpdated.Event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public ShipOwnerUpdatedEvent MapToRequest(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}