using Paramore.Brighter;
using ShipRegistryApplication;

namespace ShipRegistryPorts.Events
{
    public class LineNameUpdatedEvent : Event
    {
        public Id Lineid { get; }
        public LineName LineName { get; }
        public int Version { get; }

        public LineNameUpdatedEvent(Id lineid, LineName lineName, int version) 
            : base(lineid.Value)
        {
            Lineid = lineid;
            LineName = lineName;
            Version = version;
        }
    }
}