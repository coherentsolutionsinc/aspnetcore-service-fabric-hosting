namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderDelegateParameters<IStatefulServiceHostDelegateReplicaTemplate>,
          IServiceHostBuilderDelegateReplicationParameters<IStatefulServiceHostDelegateReplicableTemplate, IStatefulServiceHostDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerParameters<IStatefulServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
    }
}