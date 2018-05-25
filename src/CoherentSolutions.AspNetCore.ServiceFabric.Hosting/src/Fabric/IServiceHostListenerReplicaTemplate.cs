using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostListenerReplicaTemplate<out TConfigurator> : IConfigurableObject<TConfigurator>
        where TConfigurator : IServiceHostListenerReplicaTemplateConfigurator
    {
    }
}