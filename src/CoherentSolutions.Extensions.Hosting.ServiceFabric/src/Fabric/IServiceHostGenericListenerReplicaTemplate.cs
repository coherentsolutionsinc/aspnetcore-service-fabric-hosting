namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostGenericListenerReplicaTemplate<out TConfigurator>
        : IServiceHostListenerReplicaTemplate<TConfigurator>
        where TConfigurator : IServiceHostGenericListenerReplicaTemplateConfigurator
    {
    }
}