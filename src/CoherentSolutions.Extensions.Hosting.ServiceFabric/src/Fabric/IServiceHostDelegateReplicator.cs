namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateReplicator<in TService, out TDelegate>
    {
        TDelegate ReplicateFor(
            TService service);
    }
}