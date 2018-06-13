namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerParameters<IStatefulServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
    }
}