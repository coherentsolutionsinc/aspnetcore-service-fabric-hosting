using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostListenerReplicator
        : ServiceHostListenerReplicator<IStatefulServiceHostListenerReplicableTemplate, IStatefulService, ServiceReplicaListener>,
          IStatefulServiceHostListenerReplicator
    {
        public StatefulServiceHostListenerReplicator(
            IStatefulServiceHostListenerReplicableTemplate replicaTemplate)
            : base(replicaTemplate)
        {
        }
    }
}