namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        : IStatefulServiceHostListenerReplicaTemplateConfigurator,
          IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
    }
}