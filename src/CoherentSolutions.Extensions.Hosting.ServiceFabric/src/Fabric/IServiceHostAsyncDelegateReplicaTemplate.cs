using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAsyncDelegateReplicaTemplate<out TConfigurator> : IConfigurableObject<TConfigurator>
        where TConfigurator : IServiceHostAsyncDelegateReplicaTemplateConfigurator
    {
    }
}