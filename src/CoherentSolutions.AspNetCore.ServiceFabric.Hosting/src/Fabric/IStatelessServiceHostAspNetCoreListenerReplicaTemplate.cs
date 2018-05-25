namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatelessServiceHostAspNetCoreListenerReplicaTemplate
        : IStatelessServiceHostListenerReplicableTemplate,
          IServiceHostAspNetCoreListenerReplicaTemplate<IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator>

    {
    }
}