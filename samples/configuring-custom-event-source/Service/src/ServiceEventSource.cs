using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace Service
{
    [EventSource(Name = "App-Service")]
    internal sealed class ServiceEventSource : EventSource, IServiceEventSource, IApiServiceEventSource
    {
        public static class Keywords
        {
            public const EventKeywords ApiController = (EventKeywords) 0x4L;
        }

        private const int ServiceMessageEventId = 2;

        private const int GetValueMethodInvokedId = 7;

        public static readonly ServiceEventSource Current = new ServiceEventSource();

        static ServiceEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(
                () =>
                {
                });
        }

        // Instance constructor is private to enforce singleton semantics
        private ServiceEventSource()
        {
        }

        [Event(GetValueMethodInvokedId, Level = EventLevel.Informational, Message = "GetValueMethodInvoked", Keywords = Keywords.ApiController)]
        public void GetValueMethodInvoked()
        {
            this.WriteEvent(GetValueMethodInvokedId);
        }

        public void WriteEvent<T>(
            ref T eventData)
            where T : ServiceEventSourceData
        {
            this.ServiceMessage(
                eventData.ServiceName,
                eventData.ServiceTypeName,
                eventData.ReplicaOrInstanceId,
                eventData.PartitionId,
                eventData.ApplicationName,
                eventData.ApplicationTypeName,
                eventData.NodeName,
                eventData.EventMessage);
        }

        [Event(ServiceMessageEventId, Level = EventLevel.Informational, Message = "{7}")]
        private void ServiceMessage(
            string serviceName,
            string serviceTypeName,
            long replicaOrInstanceId,
            Guid partitionId,
            string applicationName,
            string applicationTypeName,
            string nodeName,
            string message)
        {
            this.WriteEvent(
                ServiceMessageEventId,
                serviceName,
                serviceTypeName,
                replicaOrInstanceId,
                partitionId,
                applicationName,
                applicationTypeName,
                nodeName,
                message);
        }
    }
}