using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatefulServiceHostDelegateReplicatorTests
        : ServiceHostDelegateReplicatorTests<IStatefulServiceHostDelegateReplicableTemplate, IStatefulService, IServiceHostDelegate>
    {
        protected override ServiceHostDelegateReplicator<IStatefulServiceHostDelegateReplicableTemplate, IStatefulService, IServiceHostDelegate>
            CreateInstance(
                IStatefulServiceHostDelegateReplicableTemplate replicableTemplate)
        {
            return new StatefulServiceHostDelegateReplicator(replicableTemplate);
        }
    }
}