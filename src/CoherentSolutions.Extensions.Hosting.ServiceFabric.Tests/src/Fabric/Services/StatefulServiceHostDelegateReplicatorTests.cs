using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric.Services
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