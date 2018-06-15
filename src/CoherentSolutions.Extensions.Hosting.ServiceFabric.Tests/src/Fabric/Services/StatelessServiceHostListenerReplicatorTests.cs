using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric.Services
{
    public class StatelessServiceHostListenerReplicatorTests
        : ServiceHostListenerReplicatorTests<IStatelessServiceHostListenerReplicableTemplate, IStatelessService, ServiceInstanceListener>
    {
        protected override ServiceHostListenerReplicator<IStatelessServiceHostListenerReplicableTemplate, IStatelessService, ServiceInstanceListener>
            CreateInstance(
                IStatelessServiceHostListenerReplicableTemplate replicableTemplate)
        {
            return new StatelessServiceHostListenerReplicator(replicableTemplate);
        }
    }
}