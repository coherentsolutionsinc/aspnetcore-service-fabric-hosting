using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostListenerReplicator
        : ServiceHostListenerReplicator<IStatelessServiceHostListenerReplicableTemplate, IStatelessService, ServiceInstanceListener>,
          IStatelessServiceHostListenerReplicator
    {
        public StatelessServiceHostListenerReplicator(
            IStatelessServiceHostListenerReplicableTemplate replicaTemplate)
            : base(replicaTemplate)
        {
        }
    }
}