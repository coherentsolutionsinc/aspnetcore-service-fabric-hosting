namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatelessServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>
    {
    }
}