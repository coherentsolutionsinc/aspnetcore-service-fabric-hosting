using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatelessService : Microsoft.ServiceFabric.Services.Runtime.StatelessService, IStatelessService
    {
        private readonly IEnumerable<IStatelessServiceHostListenerReplicator> listenerReplicators;

        public StatelessService(
            StatelessServiceContext serviceContext,
            IEnumerable<IStatelessServiceHostListenerReplicator> listenerReplicators)
            : base(serviceContext)
        {
            this.listenerReplicators = listenerReplicators
             ?? Enumerable.Empty<IStatelessServiceHostListenerReplicator>();
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.listenerReplicators.Select(replicator => replicator.ReplicateFor(this));
        }

        public IServicePartition GetPartition()
        {
            return this.Partition;
        }
    }
}