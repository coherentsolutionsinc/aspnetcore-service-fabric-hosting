namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerParameters<IStatelessServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>
    {
    }
}