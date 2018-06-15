namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAspNetCoreListenerReplicaTemplate<out TConfigurator>
        : IServiceHostListenerReplicaTemplate<TConfigurator>
        where TConfigurator : IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
    }
}