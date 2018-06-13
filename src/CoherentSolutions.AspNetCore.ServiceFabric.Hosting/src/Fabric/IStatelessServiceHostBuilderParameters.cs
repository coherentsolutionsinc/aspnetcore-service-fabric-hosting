namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatelessServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerParameters<IStatelessServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>
    {
    }
}