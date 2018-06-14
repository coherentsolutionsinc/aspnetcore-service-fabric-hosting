using System;
using System.Diagnostics.Tracing;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    [EventData]
    public class ServiceEventSourceData
    {
        [EventField]
        public int EventId { get; set; }

        [EventField]
        public string EventName { get; set; }

        [EventField]
        public string EventCategoryName { get; set; }

        [EventField]
        public string EventMessage { get; set; }

        [EventField]
        public string EventStackTrace { get; set; }

        [EventField]
        public string EventMetadata { get; set; }

        [EventField]
        public string ServiceName { get; set; }

        [EventField]
        public string ServiceTypeName { get; set; }

        [EventField]
        public long ReplicaOrInstanceId { get; set; }

        [EventField]
        public Guid PartitionId { get; set; }

        [EventField]
        public string ApplicationName { get; set; }

        [EventField]
        public string ApplicationTypeName { get; set; }

        [EventField]
        public string NodeName { get; set; }
    }
}