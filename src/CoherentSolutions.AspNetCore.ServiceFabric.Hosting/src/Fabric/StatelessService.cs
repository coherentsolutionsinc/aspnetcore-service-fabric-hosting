using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatelessService : Microsoft.ServiceFabric.Services.Runtime.StatelessService, IStatelessService
    {
        private readonly IEnumerable<IStatelessServiceHostListenerReplicator> listenerReplicators;

        private readonly ServiceEventSource eventSource;

        public StatelessService(
            StatelessServiceContext serviceContext,
            IEnumerable<IStatelessServiceHostListenerReplicator> listenerReplicators)
            : base(serviceContext)
        {
            this.eventSource = new ServiceEventSource(
                serviceContext,
                $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}",
                EventSourceSettings.Default);

            this.listenerReplicators = listenerReplicators
             ?? Enumerable.Empty<IStatelessServiceHostListenerReplicator>();
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.listenerReplicators.Select(replicator => replicator.ReplicateFor(this));
        }

        public ServiceContext GetContext()
        {
            return this.Context;
        }

        public IServiceEventSource GetEventSource()
        {
            return this.eventSource;
        }

        public IServicePartition GetPartition()
        {
            return this.Partition;
        }
    }
}