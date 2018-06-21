using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatefulServiceHostDelegateReplicatorTests
        : ServiceHostDelegateReplicatorTests<IStatefulServiceHostDelegateReplicableTemplate, IStatefulService, IServiceHostDelegateInvoker>
    {
        protected override ServiceHostDelegateReplicator<IStatefulServiceHostDelegateReplicableTemplate, IStatefulService, IServiceHostDelegateInvoker>
            CreateInstance(
                IStatefulServiceHostDelegateReplicableTemplate replicableTemplate)
        {
            return new StatefulServiceHostDelegateReplicator(replicableTemplate);
        }
    }
}