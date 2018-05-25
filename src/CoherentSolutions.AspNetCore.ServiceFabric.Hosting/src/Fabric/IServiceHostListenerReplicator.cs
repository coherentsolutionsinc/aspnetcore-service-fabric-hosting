namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostListenerReplicator<in TService, out TListener>
    {
        TListener ReplicateFor(
            TService service);
    }
}