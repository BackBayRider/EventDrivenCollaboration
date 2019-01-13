using Paramore.Brighter;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Events
{
    public class NewLineAddedEvent : Event
    {
        public Id LineId { get; } 
        public LineName LineName { get; }

        public NewLineAddedEvent(Id lineId, LineName lineName)
            :base (lineId.Value)
        {
            LineId = lineId;
            LineName = lineName;
        }
    }
}