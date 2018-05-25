namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostAspNetCoreListenerReplicaTemplate
        : IStatefulServiceHostListenerReplicableTemplate,
          IServiceHostAspNetCoreListenerReplicaTemplate<IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator>

    {
    }
}