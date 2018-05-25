namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostAspNetCoreListenerReplicaTemplate<out TConfigurator>
        : IServiceHostListenerReplicaTemplate<TConfigurator>
        where TConfigurator : IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
    }
}