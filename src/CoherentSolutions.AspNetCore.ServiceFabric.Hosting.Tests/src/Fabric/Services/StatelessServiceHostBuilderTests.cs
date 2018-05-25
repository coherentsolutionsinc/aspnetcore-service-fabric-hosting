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
            IStatelessServiceHostListenerReplicator
        >
    {
        protected override ServiceHostBuilder<
                IStatelessServiceHost,
                IStatelessServiceHostBuilderParameters,
                IStatelessServiceHostBuilderConfigurator,
                IStatelessServiceHostListenerReplicableTemplate,
                IStatelessServiceHostAspNetCoreListenerReplicaTemplate,
                IStatelessServiceHostListenerReplicator
            >
            CreateInstance()
        {
            return new StatelessServiceHostBuilder();
        }
    }
}