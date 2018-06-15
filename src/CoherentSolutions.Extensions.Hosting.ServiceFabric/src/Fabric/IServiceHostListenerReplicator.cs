namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicator<in TService, out TListener>
    {
        TListener ReplicateFor(
            TService service);
    }
}