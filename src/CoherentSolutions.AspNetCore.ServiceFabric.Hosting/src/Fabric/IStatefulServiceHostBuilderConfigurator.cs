namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
    }
}