using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicaTemplate<out TConfigurator> : IConfigurableObject<TConfigurator>
        where TConfigurator : IServiceHostEventSourceReplicaTemplateConfigurator
    {
        
    }
}