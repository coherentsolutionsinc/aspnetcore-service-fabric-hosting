namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderDelegateConfigurator<IStatefulServiceHostDelegateReplicaTemplate>,
          IServiceHostBuilderDelegateReplicationConfigurator<IStatefulServiceHostDelegateReplicableTemplate, IStatefulServiceHostDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerConfigurator<IStatefulServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
    }
}