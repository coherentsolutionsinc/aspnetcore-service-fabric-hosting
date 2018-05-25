namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatelessServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>

    {
    }
}