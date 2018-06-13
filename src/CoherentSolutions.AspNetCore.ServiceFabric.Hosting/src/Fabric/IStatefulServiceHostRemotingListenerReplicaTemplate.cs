namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostRemotingListenerReplicaTemplate
        : IStatefulServiceHostListenerReplicableTemplate,
          IServiceHostRemotingListenerReplicaTemplate<IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator>

    {
    }
}