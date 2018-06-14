namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatelessServiceHostRemotingListenerReplicaTemplate
        : IStatelessServiceHostListenerReplicableTemplate,
          IServiceHostRemotingListenerReplicaTemplate<IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator>

    {
    }
}