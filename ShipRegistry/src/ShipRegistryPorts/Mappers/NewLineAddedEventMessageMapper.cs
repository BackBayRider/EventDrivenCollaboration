using Newtonsoft.Json;
using Paramore.Brighter;
using ShipRegistryPorts.Events;

namespace ShipRegistryPorts.Mappers
{
    public class NewLineAddedEventMessageMapper : IAmAMessageMapper<NewLineAddedEvent>
    {
        public Message MapToMessage(NewLineAddedEvent request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "ShipRegistration.ShippingLine.NewLineAdded.Event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public NewLineAddedEvent MapToRequest(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}