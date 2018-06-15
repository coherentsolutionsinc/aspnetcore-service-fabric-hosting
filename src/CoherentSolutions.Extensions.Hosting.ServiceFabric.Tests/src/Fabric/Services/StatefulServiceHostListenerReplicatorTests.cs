using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric.Services
{
    public class StatefulServiceHostListenerReplicatorTests
        : ServiceHostListenerReplicatorTests<IStatefulServiceHostListenerReplicableTemplate, IStatefulService, ServiceReplicaListener>
    {
        protected override ServiceHostListenerReplicator<IStatefulServiceHostListenerReplicableTemplate, IStatefulService, ServiceReplicaListener>
            CreateInstance(
                IStatefulServiceHostListenerReplicableTemplate replicableTemplate)
        {
            return new StatefulServiceHostListenerReplicator(replicableTemplate);
        }
    }
}