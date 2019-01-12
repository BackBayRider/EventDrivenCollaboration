using Paramore.Brighter;
using ShipRegistryCore.Application;

namespace ShipRegistryCore.Ports.Events
{
    public class LineNameUpdatedEvent : Event
    {
        public Id Lineid { get; }
        public LineName LineName { get; }

        public LineNameUpdatedEvent(Id lineid, LineName lineName) 
            : base(lineid.Value)
        {
            Lineid = lineid;
            LineName = lineName;
        }
    }
}