using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects.Base;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
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