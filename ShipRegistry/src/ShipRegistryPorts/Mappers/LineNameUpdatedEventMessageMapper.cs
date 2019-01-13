using Newtonsoft.Json;
using Paramore.Brighter;
using ShipRegistryPorts.Events;

namespace ShipRegistryPorts.Mappers
{
    public class LineNameUpdatedEventMessageMapper : IAmAMessageMapper<LineNameUpdatedEvent>
    {
        public Message MapToMessage(LineNameUpdatedEvent request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "ShipRegistration.ShippingLine.LineNameUpdated.Event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public LineNameUpdatedEvent MapToRequest(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}