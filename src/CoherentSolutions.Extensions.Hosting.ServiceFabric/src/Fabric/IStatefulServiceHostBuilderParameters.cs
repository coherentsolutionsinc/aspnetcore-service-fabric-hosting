namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderAsyncDelegateParameters<IStatefulServiceHostAsyncDelegateReplicaTemplate>,
          IServiceHostBuilderAsyncDelegateReplicationParameters<IStatefulServiceHostAsyncDelegateReplicableTemplate, IStatefulServiceHostAsyncDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerParameters<IStatefulServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
    }
}