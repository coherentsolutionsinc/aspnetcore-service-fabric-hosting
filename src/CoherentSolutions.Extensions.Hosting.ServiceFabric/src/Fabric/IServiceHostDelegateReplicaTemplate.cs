using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateReplicaTemplate<out TConfigurator> : IConfigurableObject<TConfigurator>
        where TConfigurator : IServiceHostDelegateReplicaTemplateConfigurator
    {
    }
}