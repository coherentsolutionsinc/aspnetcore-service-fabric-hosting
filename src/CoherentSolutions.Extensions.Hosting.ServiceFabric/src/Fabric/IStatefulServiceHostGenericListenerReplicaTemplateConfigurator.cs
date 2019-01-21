namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostGenericListenerReplicaTemplateConfigurator
        : IStatefulServiceHostListenerReplicaTemplateConfigurator,
          IServiceHostGenericListenerReplicaTemplateConfigurator
    {
    }
}