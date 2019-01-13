using Newtonsoft.Json;
using Paramore.Brighter;
using ShipRegistryPorts.Events;

namespace ShipRegistryPorts.Mappers
{
    public class NewShipAddedEventMessageMapper : IAmAMessageMapper<NewShipAddedEvent>
    {
        public Message MapToMessage(NewShipAddedEvent request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "ShipRegistration.Ship.NewShipAdded.Event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public NewShipAddedEvent MapToRequest(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}