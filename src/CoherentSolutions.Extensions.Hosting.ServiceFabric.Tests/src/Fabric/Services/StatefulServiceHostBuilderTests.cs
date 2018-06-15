using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric.Services
{
    public class StatefulServiceHostBuilderTests
        : ServiceHostBuilderTests<IStatefulServiceHost,
            IStatefulServiceHostBuilderParameters,
            IStatefulServiceHostBuilderConfigurator,
            IStatefulServiceHostListenerReplicableTemplate,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            IStatefulServiceHostRemotingListenerReplicaTemplate,
            IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator,
            IStatefulServiceHostListenerReplicator
        >
    {
        protected override ServiceHostBuilder<
                IStatefulServiceHost,
                IStatefulServiceHostBuilderParameters,
                IStatefulServiceHostBuilderConfigurator,
                IStatefulServiceHostListenerReplicableTemplate,
                IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
                IStatefulServiceHostRemotingListenerReplicaTemplate,
                IStatefulServiceHostListenerReplicator
            >
            CreateInstance()
        {
            return new StatefulServiceHostBuilder();
        }
    }
}