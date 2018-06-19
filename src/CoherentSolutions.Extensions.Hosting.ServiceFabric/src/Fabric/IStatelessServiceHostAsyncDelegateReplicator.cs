namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostAsyncDelegateReplicator
        : IServiceHostAsyncDelegateReplicator<IStatelessService, IServiceHostAsyncDelegate>
    {
    }
}