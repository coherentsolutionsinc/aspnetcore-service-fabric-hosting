using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatefulService : Microsoft.ServiceFabric.Services.Runtime.StatefulService, IStatefulService
    {
        private readonly IEnumerable<IStatefulServiceHostListenerReplicator> listenerReplicators;

        public StatefulService(
            StatefulServiceContext serviceContext,
            IEnumerable<IStatefulServiceHostListenerReplicator> listenerReplicators)
            : base(serviceContext)
        {
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

        public IServicePartition GetPartition()
        {
            return this.Partition;
        }
    }
}