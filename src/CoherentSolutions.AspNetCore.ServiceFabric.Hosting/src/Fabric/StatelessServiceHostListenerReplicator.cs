using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
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