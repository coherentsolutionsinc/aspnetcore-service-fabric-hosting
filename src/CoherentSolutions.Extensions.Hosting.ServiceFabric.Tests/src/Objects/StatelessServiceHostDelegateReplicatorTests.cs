using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatelessServiceHostDelegateReplicatorTests
        : ServiceHostDelegateReplicatorTests<IStatelessServiceHostDelegateReplicableTemplate, IStatelessService, IServiceHostDelegateInvoker>
    {
        protected override ServiceHostDelegateReplicator<IStatelessServiceHostDelegateReplicableTemplate, IStatelessService, IServiceHostDelegateInvoker>
            CreateInstance(
                IStatelessServiceHostDelegateReplicableTemplate replicableTemplate)
        {
            return new StatelessServiceHostDelegateReplicator(replicableTemplate);
        }
    }
}