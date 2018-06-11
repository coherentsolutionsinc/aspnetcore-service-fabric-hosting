using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;

using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatefulService : Microsoft.ServiceFabric.Services.Runtime.StatefulService, IStatefulService
    {
        private readonly IEnumerable<IStatefulServiceHostListenerReplicator> listenerReplicators;

        private readonly ServiceEventSource eventSource;

        public StatefulService(
            StatefulServiceContext serviceContext,
            IEnumerable<IStatefulServiceHostListenerReplicator> listenerReplicators)
            : base(serviceContext)
        {
            this.eventSource = new ServiceEventSource(
                serviceContext,
                $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}",
                EventSourceSettings.EtwSelfDescribingEventFormat);

            this.listenerReplicators = listenerReplicators
             ?? Enumerable.Empty<IStatefulServiceHostListenerReplicator>();
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.listenerReplicators.Select(replicator => replicator.ReplicateFor(this));
        }

        public IReliableStateManager GetReliableStateManager()
        {
            return this.StateManager;
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