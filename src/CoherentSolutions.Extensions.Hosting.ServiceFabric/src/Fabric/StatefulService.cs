using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;

using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulService : Microsoft.ServiceFabric.Services.Runtime.StatefulService, IStatefulService
    {
        private readonly IServiceProvider serviceDependencies;

        private readonly IEnumerable<IStatefulServiceHostListenerReplicator> serviceListenerReplicators;

        private readonly ServiceEventSource eventSource;

        public StatefulService(
            StatefulServiceContext serviceContext,
            IServiceProvider serviceDependencies,
            IEnumerable<IStatefulServiceHostListenerReplicator> serviceListenerReplicators)
            : base(serviceContext)
        {
            this.eventSource = new ServiceEventSource(
                serviceContext,
                $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}",
                EventSourceSettings.EtwSelfDescribingEventFormat);

            this.serviceDependencies = serviceDependencies 
             ?? throw new ArgumentNullException(nameof(serviceDependencies));

            this.serviceListenerReplicators = serviceListenerReplicators
             ?? Enumerable.Empty<IStatefulServiceHostListenerReplicator>();
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.serviceListenerReplicators.Select(replicator => replicator.ReplicateFor(this));
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