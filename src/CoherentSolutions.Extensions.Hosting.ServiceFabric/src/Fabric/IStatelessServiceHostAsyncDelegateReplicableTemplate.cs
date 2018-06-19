namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostAsyncDelegateReplicableTemplate
        : IServiceHostAsyncDelegateReplicableTemplate<IStatelessService, IServiceHostAsyncDelegate>
    {
    }
}