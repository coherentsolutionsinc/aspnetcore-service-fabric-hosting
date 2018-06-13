using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Fabric.Services
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