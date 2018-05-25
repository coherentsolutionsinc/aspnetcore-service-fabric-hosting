namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
    }
}