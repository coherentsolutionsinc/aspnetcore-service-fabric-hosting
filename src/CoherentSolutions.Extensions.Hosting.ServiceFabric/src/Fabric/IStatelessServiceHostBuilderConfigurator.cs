namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderDelegateConfigurator<IStatelessServiceHostDelegateReplicaTemplate>,
          IServiceHostBuilderDelegateReplicationConfigurator<IStatelessServiceHostDelegateReplicableTemplate, IStatelessServiceHostDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerConfigurator<IStatelessServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>

    {
    }
}