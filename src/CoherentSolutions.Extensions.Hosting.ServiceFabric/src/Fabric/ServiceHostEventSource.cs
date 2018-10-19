using System.Diagnostics.Tracing;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostEventSource : EventSource, IServiceEventSource
    {
        private static class Keywords
        {
            public const EventKeywords APPLICATION_TRACE = (EventKeywords) 0x1L;
        }

        private readonly ServiceContext serviceContext;

        public ServiceHostEventSource(
            ServiceContext serviceContext,
            string eventSourceName,
            EventSourceSettings eventSourceConfig)
            : base(eventSourceName, eventSourceConfig)
        {
            this.serviceContext = serviceContext;
        }

        public bool IsEnabled(
            EventLevel eventLevel)
        {
            return this.IsEnabled(eventLevel, Keywords.APPLICATION_TRACE);
        }

        [NonEvent]
        public void Verbose<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
            this.ServiceMessage(eventId, eventName, eventCategoryName, eventMessage, EventLevel.Verbose, EventOpcode.Info, eventData);
        }

        [NonEvent]
        public void Information<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
            this.ServiceMessage(eventId, eventName, eventCategoryName, eventMessage, EventLevel.Informational, EventOpcode.Info, eventData);
        }

        [NonEvent]
        public void Warning<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
            this.ServiceMessage(eventId, eventName, eventCategoryName, eventMessage, EventLevel.Warning, EventOpcode.Info, eventData);
        }

        [NonEvent]
        public void Error<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
            this.ServiceMessage(eventId, eventName, eventCategoryName, eventMessage, EventLevel.Error, EventOpcode.Info, eventData);
        }

        [NonEvent]
        public void Critical<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            T eventData)
            where T : ServiceEventSourceData
        {
            this.ServiceMessage(eventId, eventName, eventCategoryName, eventMessage, EventLevel.Critical, EventOpcode.Info, eventData);
        }

        [NonEvent]
        private void ServiceMessage<T>(
            int eventId,
            string eventName,
            string eventCategoryName,
            string eventMessage,
            EventLevel eventLevel,
            EventOpcode eventOpcode,
            T eventData)
            where T : ServiceEventSourceData
        {
            if (!this.IsEnabled(eventLevel, Keywords.APPLICATION_TRACE))
            {
                return;
            }

            eventData.EventId = eventId;
            eventData.EventName = eventName;
            eventData.EventCategoryName = eventCategoryName;
            eventData.EventMessage = eventMessage;
            eventData.ServiceName = this.serviceContext.ServiceName.AbsoluteUri;
            eventData.ServiceTypeName = this.serviceContext.ServiceTypeName;
            eventData.ReplicaOrInstanceId = this.serviceContext.ReplicaOrInstanceId;
            eventData.PartitionId = this.serviceContext.PartitionId;
            eventData.ApplicationName = this.serviceContext.CodePackageActivationContext.ApplicationName;
            eventData.ApplicationTypeName = this.serviceContext.CodePackageActivationContext.ApplicationTypeName;
            eventData.NodeName = this.serviceContext.NodeContext.NodeName;

            this.Write(
                eventName,
                new EventSourceOptions
                {
                    Level = eventLevel,
                    Opcode = eventOpcode,
                    Keywords = Keywords.APPLICATION_TRACE
                },
                eventData);
        }
    }
}