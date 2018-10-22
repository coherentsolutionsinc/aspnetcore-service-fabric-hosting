using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace Service
{
    [EventSource(Name = "App-StatefulService")]
    internal sealed class StatefulServiceEventSource : EventSource
    {
        public static class Keywords
        {
            public const EventKeywords ServiceLifecycle = (EventKeywords) 0x4L;
        }

        private const int ServiceReplicaStartupEventId = 1;

        private const int ServiceReplicaChangeRoleEventId = 2;

        private const int ServiceReplicaRunEventId = 3;

        private const int ServiceReplicaShutdownEventId = 4;

        private const int ServiceReplicaDataLossEventId = 5;

        private const int ServiceReplicaRestoreCompletedEventId = 6;

        public static readonly StatefulServiceEventSource Current = new StatefulServiceEventSource();

        static StatefulServiceEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(
                () =>
                {
                });
        }

        // Instance constructor is private to enforce singleton semantics
        private StatefulServiceEventSource()
        {
        }

        [Event(
            ServiceReplicaStartupEventId,
            Level = EventLevel.Informational,
            Message = "The replica {0} is starting up.",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceReplicaStartupEvent(
            long replicaId)
        {
            this.WriteEvent(ServiceReplicaStartupEventId, replicaId);
        }

        [Event(
            ServiceReplicaChangeRoleEventId,
            Level = EventLevel.Informational,
            Message = "The replica {0} is changing role: {1}.",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceReplicaChangeRoleEvent(
            long replicaId,
            string newRole)
        {
            this.WriteEvent(ServiceReplicaChangeRoleEventId, replicaId, newRole);
        }

        [Event(
            ServiceReplicaRunEventId,
            Level = EventLevel.Informational,
            Message = "The replica {0} is running (primary).",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceReplicaRunEvent(
            long replicaId)
        {
            this.WriteEvent(ServiceReplicaRunEventId, replicaId);
        }

        [Event(
            ServiceReplicaShutdownEventId,
            Level = EventLevel.Informational,
            Message = "The replica {0} is shutting down (aborting: {1}).",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceReplicaShutdownEvent(
            long replicaId,
            bool isAborting)
        {
            this.WriteEvent(ServiceReplicaShutdownEventId, replicaId, isAborting);
        }

        [Event(
            ServiceReplicaDataLossEventId,
            Level = EventLevel.Informational,
            Message = "The replica {0} data loss detected.",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceReplicaDataLossEvent(
            long replicaId)
        {
            this.WriteEvent(ServiceReplicaDataLossEventId, replicaId);
        }

        [Event(
            ServiceReplicaRestoreCompletedEventId,
            Level = EventLevel.Informational,
            Message = "The replica {0} restore completed.",
            Keywords = Keywords.ServiceLifecycle)]
        public void ServiceReplicaRestoreCompletedEvent(
            long replicaId)
        {
            this.WriteEvent(ServiceReplicaRestoreCompletedEventId, replicaId);
        }
    }
}