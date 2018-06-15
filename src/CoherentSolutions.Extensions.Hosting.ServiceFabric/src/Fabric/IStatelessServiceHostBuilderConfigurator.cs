namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerConfigurator<IStatelessServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>

    {
    }
}