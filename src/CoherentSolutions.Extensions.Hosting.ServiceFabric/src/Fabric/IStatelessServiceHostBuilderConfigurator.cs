namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderAsyncDelegateConfigurator<IStatelessServiceHostAsyncDelegateReplicaTemplate>,
          IServiceHostBuilderAsyncDelegateReplicationConfigurator<IStatelessServiceHostAsyncDelegateReplicableTemplate, IStatelessServiceHostAsyncDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerConfigurator<IStatelessServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>

    {
    }
}