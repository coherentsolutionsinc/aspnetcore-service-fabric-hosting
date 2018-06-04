using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceEventSourceData
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string EventCategoryName { get; set; }

        public string EventMessage { get; set; }

        public string ServiceName { get; set; }

        public string ServiceTypeName { get; set; }

        public long ReplicaOrInstanceId { get; set; }

        public Guid PartitionId { get; set; }

        public string ApplicationName { get; set; }

        public string ApplicationTypeName { get; set; }

        public string NodeName { get; set; }
    }
}