using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatelessServiceHostDelegateReplicatorTests
        : ServiceHostDelegateReplicatorTests<IStatelessServiceHostDelegateReplicableTemplate, IStatelessService, IServiceHostDelegate>
    {
        protected override ServiceHostDelegateReplicator<IStatelessServiceHostDelegateReplicableTemplate, IStatelessService, IServiceHostDelegate>
            CreateInstance(
                IStatelessServiceHostDelegateReplicableTemplate replicableTemplate)
        {
            return new StatelessServiceHostDelegateReplicator(replicableTemplate);
        }
    }
}