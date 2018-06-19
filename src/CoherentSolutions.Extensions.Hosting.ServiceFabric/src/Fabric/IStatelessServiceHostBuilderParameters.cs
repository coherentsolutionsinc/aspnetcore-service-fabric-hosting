namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderAsyncDelegateParameters<IStatelessServiceHostAsyncDelegateReplicaTemplate>,
          IServiceHostBuilderAsyncDelegateReplicationParameters<IStatelessServiceHostAsyncDelegateReplicableTemplate, IStatelessServiceHostAsyncDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerParameters<IStatelessServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>
    {
    }
}