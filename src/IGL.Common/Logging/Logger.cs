using Microsoft.Diagnostics.Tracing;

namespace IGL.Logging
{
    public sealed class Logger : EventSource
    {
        [Event(500, Level = EventLevel.Verbose)]
        public void LogVerbose(string message)
        {
            Write(message);
        }

        [Event(400, Level = EventLevel.Informational)]
        public void LogInformation(string message)
        {
            Write(message);
        }

        [Event(300, Level = EventLevel.Warning)]
        public void LogWarning(string message)
        {
            Write(message);
        }

        [Event(200, Level = EventLevel.Error)]
        public void LogError(string message)
        {
            Write(message);
        }
    }
}
