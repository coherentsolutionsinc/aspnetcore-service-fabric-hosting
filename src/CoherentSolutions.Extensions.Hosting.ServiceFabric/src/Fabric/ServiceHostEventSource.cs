using System;
using System.Diagnostics.Tracing;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public sealed class ServiceHostEventSource : EventSource, IServiceEventSource
    {
        private static class Keywords
        {
            public const EventKeywords APPLICATION_TRACE = (EventKeywords) 0x1L;
        }

        public ServiceHostEventSource(
            ServiceContext serviceContext)
            : base(CreateEventSourceName(serviceContext), EventSourceSettings.EtwSelfDescribingEventFormat)
        {
        }

        [NonEvent]
        public void WriteEvent<T>(
            ref T eventData)
            where T : ServiceEventSourceData
        {
            this.Write(
                eventData.EventName,
                new EventSourceOptions
                {
                    Level = eventData.EventLevel,
                    Opcode = eventData.EventOpCode,
                    Keywords = Keywords.APPLICATION_TRACE
                },
                eventData);
        }

        private static string CreateEventSourceName(
            ServiceContext serviceContext)
        {
            if (serviceContext is null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }
            if (serviceContext.CodePackageActivationContext is null)
            {
                throw new ArgumentException(nameof(serviceContext.CodePackageActivationContext));
            }

            return $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}";
        }
    }
}