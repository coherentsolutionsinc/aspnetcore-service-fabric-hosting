namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostAspNetCoreListenerReplicaTemplate
        : IStatelessServiceHostListenerReplicableTemplate,
          IServiceHostAspNetCoreListenerReplicaTemplate<IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator>

    {
    }
}