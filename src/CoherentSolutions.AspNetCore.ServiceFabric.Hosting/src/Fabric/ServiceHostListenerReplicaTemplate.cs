using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public abstract class ServiceHostListenerReplicaTemplate<TConfigurator>
        : ConfigurableObject<TConfigurator>, IServiceHostListenerReplicaTemplate<TConfigurator>
        where TConfigurator : IServiceHostListenerReplicaTemplateConfigurator
    {
    }
}