using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace Service
{
    [EventSource(Name = "App-StatelessService")]
    internal sealed class StatelessServiceEventSource : EventSource
    {
        // Event keywords can be used to categorize events. 
        // Each keyword is a bit flag. A single event can be associated with multiple keywords (via EventAttribute.Keywords property).
        // Keywords must be defined as a public class named 'Keywords' inside EventSource that uses them.
        public static class Keywords
        {
            public const EventKeywords ServiceLifecycle = (EventKeywords) 0x4L;
        }

        private const int ServiceInstanceStartupEventId = 1;

        private const int ServiceInstanceRunEventId = 2;

        private const int ServiceInstanceShutdownEventId = 3;

        public static readonly StatelessServiceEventSource Current = new StatelessServiceEventSource();

        static StatelessServiceEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(
                () =>
                {
                });
        }

        // Instance constructor is private to enforce singleton semantics
        private StatelessServiceEventSource()
        {
        }

        [Event(
            ServiceInstanceStartupEventId,
            Level = EventLevel.Informational,
            Message = "The instance {0} is starting up.",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceInstanceStartupEvent(
            long instanceId)
        {
            this.WriteEvent(ServiceInstanceStartupEventId, instanceId);
        }

        [Event(ServiceInstanceRunEventId, Level = EventLevel.Informational, Message = "The instance {0} is running.", Keywords = Keywords.ServiceLifecycle)]
        public void ServiceInstanceRunEvent(
            long instanceId)
        {
            this.WriteEvent(ServiceInstanceRunEventId, instanceId);
        }

        [Event(
            ServiceInstanceShutdownEventId,
            Level = EventLevel.Informational,
            Message = "The instance {0} is shutting down (aborting: {1}).",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceInstanceShutdownEvent(
            long instanceId,
            bool isAborting)
        {
            this.WriteEvent(ServiceInstanceShutdownEventId, instanceId, isAborting);
        }
    }
}