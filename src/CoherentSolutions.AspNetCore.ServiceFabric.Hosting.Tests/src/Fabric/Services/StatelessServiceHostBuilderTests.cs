using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Fabric.Services
{
    public class StatelessServiceHostBuilderTests
        : ServiceHostBuilderTests<IStatelessServiceHost,
            IStatelessServiceHostBuilderParameters,
            IStatelessServiceHostBuilderConfigurator,
            IStatelessServiceHostListenerReplicableTemplate,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplate,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            IStatelessServiceHostRemotingListenerReplicaTemplate,
            IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator,
            IStatelessServiceHostListenerReplicator
        >
    {
        protected override ServiceHostBuilder<
                IStatelessServiceHost,
                IStatelessServiceHostBuilderParameters,
                IStatelessServiceHostBuilderConfigurator,
                IStatelessServiceHostListenerReplicableTemplate,
                IStatelessServiceHostAspNetCoreListenerReplicaTemplate,
                IStatelessServiceHostRemotingListenerReplicaTemplate,
                IStatelessServiceHostListenerReplicator
            >
            CreateInstance()
        {
            return new StatelessServiceHostBuilder();
        }
    }
}