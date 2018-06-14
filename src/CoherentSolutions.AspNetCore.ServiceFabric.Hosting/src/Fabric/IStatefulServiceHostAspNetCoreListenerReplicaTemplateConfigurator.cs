namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        : IStatefulServiceListenerReplicaTemplateConfigurator,
          IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
    }
}