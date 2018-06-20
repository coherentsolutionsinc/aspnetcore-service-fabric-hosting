using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric.Services
{
    public class StatelessServiceHostBuilderTests
        : ServiceHostBuilderTests<IStatelessServiceHost,
            IStatelessServiceHostBuilderParameters,
            IStatelessServiceHostBuilderConfigurator,
            IStatelessServiceHostDelegateReplicableTemplate,
            IStatelessServiceHostDelegateReplicaTemplate,
            IStatelessServiceHostDelegateReplicaTemplateConfigurator,
            IStatelessServiceHostDelegateReplicator,
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
                IStatelessServiceHostDelegateReplicableTemplate,
                IStatelessServiceHostDelegateReplicaTemplate,
                IStatelessServiceHostDelegateReplicator,
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