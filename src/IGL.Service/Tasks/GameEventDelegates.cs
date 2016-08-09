using System;

namespace IGL.Service.Tasks
{
    public enum ProcessEventType
    {
        GameEvent,    // GameEvent successfully processed
        SteamEvent   // SteamEvent successfully processed        
    }

    public class ProcessingEvent
    {
        public DateTime ProcessedDateTime { get; set; }
        public ProcessEventType EventType { get; set; }
        public bool HasFailed { get; set; }        
    }    
}
