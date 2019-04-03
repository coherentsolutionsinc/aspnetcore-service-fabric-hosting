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

        private const int ServiceReplicaCodePackageEventId = 7;

        private const int ServiceReplicaConfigPackageEventId = 8;

        private const int ServiceReplicaDataPackageEventId = 9;

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

        [Event(
            ServiceReplicaCodePackageEventId,
            Level = EventLevel.Informational,
            Message = "Code Package {0}.",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceReplicaCodePackageEvent(
            string ev)
        {
            this.WriteEvent(ServiceReplicaCodePackageEventId, ev);
        }

        [Event(
            ServiceReplicaConfigPackageEventId,
            Level = EventLevel.Informational,
            Message = "Config Package {0}.",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceReplicaConfigPackageEvent(
            string ev)
        {
            this.WriteEvent(ServiceReplicaConfigPackageEventId, ev);
        }

        [Event(
            ServiceReplicaDataPackageEventId,
            Level = EventLevel.Informational,
            Message = "Data Package {0}.",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceReplicaDataPackageEvent(
            string ev)
        {
            this.WriteEvent(ServiceReplicaDataPackageEventId, ev);
        }
    }
}