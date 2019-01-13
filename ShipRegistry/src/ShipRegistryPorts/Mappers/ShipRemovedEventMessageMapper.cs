using Newtonsoft.Json;
using Paramore.Brighter;
using ShipRegistryPorts.Events;

namespace ShipRegistryPorts.Mappers
{
    public class ShipRemovedEventMessageMapper : IAmAMessageMapper<ShipRemovedEvent>
    {
        public Message MapToMessage(ShipRemovedEvent request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "ShipRegistration.Ship.ShipRemoved.Event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public ShipRemovedEvent MapToRequest(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}