namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostAsyncDelegateReplicator
        : IServiceHostAsyncDelegateReplicator<IStatefulService, IServiceHostAsyncDelegate>
    {
    }
}