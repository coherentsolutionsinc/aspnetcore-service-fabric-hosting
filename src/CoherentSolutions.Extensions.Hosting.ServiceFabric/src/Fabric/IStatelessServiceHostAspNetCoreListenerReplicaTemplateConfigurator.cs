namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        : IStatelessServiceHostListenerReplicaTemplateConfigurator,
          IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
    }
}