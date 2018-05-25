namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostListenerReplicableTemplate<in TService, out TListener>
    {
        TListener Activate(
            TService service);
    }
}