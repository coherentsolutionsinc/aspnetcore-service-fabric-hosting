using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Fabric.Services
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