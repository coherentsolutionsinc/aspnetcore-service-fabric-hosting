namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator
        : IStatefulServiceHostListenerReplicaTemplateConfigurator,
          IServiceHostRemotingListenerReplicaTemplateConfigurator
    {
    }
}