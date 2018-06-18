using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessService : Microsoft.ServiceFabric.Services.Runtime.StatelessService, IStatelessService
    {
        private readonly IServiceProvider serviceDependencies;

        private readonly IEnumerable<IStatelessServiceHostListenerReplicator> serviceListenerReplicators;

        private readonly ServiceEventSource eventSource;

        public StatelessService(
            StatelessServiceContext serviceContext,
            IServiceProvider serviceDependencies,
            IEnumerable<IStatelessServiceHostListenerReplicator> serviceListenerReplicators)
            : base(serviceContext)
        {
            this.eventSource = new ServiceEventSource(
                serviceContext,
                $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}",
                EventSourceSettings.EtwSelfDescribingEventFormat);

            this.serviceDependencies = serviceDependencies 
             ?? throw new ArgumentNullException(nameof(serviceDependencies));

            this.serviceListenerReplicators = serviceListenerReplicators
             ?? Enumerable.Empty<IStatelessServiceHostListenerReplicator>();
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.serviceListenerReplicators.Select(replicator => replicator.ReplicateFor(this));
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