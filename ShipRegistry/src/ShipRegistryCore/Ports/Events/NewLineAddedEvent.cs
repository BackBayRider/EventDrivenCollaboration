using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Events
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