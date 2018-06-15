namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostRemotingListenerReplicaTemplate
        : IStatefulServiceHostListenerReplicableTemplate,
          IServiceHostRemotingListenerReplicaTemplate<IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator>

    {
    }
}