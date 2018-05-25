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
            IStatefulServiceHostListenerReplicator
        >
    {
        protected override ServiceHostBuilder<
                IStatefulServiceHost,
                IStatefulServiceHostBuilderParameters,
                IStatefulServiceHostBuilderConfigurator,
                IStatefulServiceHostListenerReplicableTemplate,
                IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
                IStatefulServiceHostListenerReplicator
            >
            CreateInstance()
        {
            return new StatefulServiceHostBuilder();
        }
    }
}