namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostAspNetCoreListenerReplicaTemplate
        : IStatefulServiceHostListenerReplicableTemplate,
          IServiceHostAspNetCoreListenerReplicaTemplate<IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator>

    {
    }
}