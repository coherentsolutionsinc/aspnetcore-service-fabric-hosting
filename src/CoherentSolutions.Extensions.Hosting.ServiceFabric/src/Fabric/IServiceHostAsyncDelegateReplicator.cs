namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAsyncDelegateReplicator<in TService, out TDelegate>
    {
        TDelegate ReplicateFor(
            TService service);
    }
}