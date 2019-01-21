namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostGenericListenerReplicaTemplate
        : IStatefulServiceHostListenerReplicableTemplate,
          IServiceHostGenericListenerReplicaTemplate<IStatefulServiceHostGenericListenerReplicaTemplateConfigurator>
    {
    }
}