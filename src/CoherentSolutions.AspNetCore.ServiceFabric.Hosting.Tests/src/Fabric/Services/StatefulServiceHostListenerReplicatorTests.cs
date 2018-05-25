using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Fabric.Services
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