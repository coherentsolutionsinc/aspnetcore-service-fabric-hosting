using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects.Base;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
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