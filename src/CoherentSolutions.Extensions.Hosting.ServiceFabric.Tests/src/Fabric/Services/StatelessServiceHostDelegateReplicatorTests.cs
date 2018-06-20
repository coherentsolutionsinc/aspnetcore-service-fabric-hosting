using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric.Services
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