using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Events
{
    public class NewLineAddedEvent
    {
        public Id LineId { get; } 
        public LineName LineName { get; }

        public NewLineAddedEvent(Id lineId, LineName lineName)
        {
            LineId = lineId;
            LineName = lineName;
        }
    }
}