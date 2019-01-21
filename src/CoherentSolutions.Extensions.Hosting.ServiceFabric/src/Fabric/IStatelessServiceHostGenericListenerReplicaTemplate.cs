namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostGenericListenerReplicaTemplate
        : IStatelessServiceHostListenerReplicableTemplate,
          IServiceHostGenericListenerReplicaTemplate<IStatelessServiceHostGenericListenerReplicaTemplateConfigurator>
    {
    }
}