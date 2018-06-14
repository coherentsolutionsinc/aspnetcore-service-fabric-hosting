namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator
        : IStatefulServiceListenerReplicaTemplateConfigurator,
          IServiceHostRemotingListenerReplicaTemplateConfigurator
    {
    }
}