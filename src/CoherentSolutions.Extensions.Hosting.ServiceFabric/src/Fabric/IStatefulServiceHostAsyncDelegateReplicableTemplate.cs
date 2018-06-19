namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostAsyncDelegateReplicableTemplate
        : IServiceHostAsyncDelegateReplicableTemplate<IStatefulService, IServiceHostAsyncDelegate>
    {
    }
}