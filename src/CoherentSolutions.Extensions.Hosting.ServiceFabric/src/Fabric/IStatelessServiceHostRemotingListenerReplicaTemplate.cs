namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostRemotingListenerReplicaTemplate
        : IStatelessServiceHostListenerReplicableTemplate,
          IServiceHostRemotingListenerReplicaTemplate<IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator>

    {
    }
}