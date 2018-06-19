namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderAsyncDelegateConfigurator<IStatefulServiceHostAsyncDelegateReplicaTemplate>,
          IServiceHostBuilderAsyncDelegateReplicationConfigurator<IStatefulServiceHostAsyncDelegateReplicableTemplate, IStatefulServiceHostAsyncDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerConfigurator<IStatefulServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
    }
}