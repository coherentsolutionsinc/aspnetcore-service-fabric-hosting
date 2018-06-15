namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicableTemplate<in TService, out TListener>
    {
        TListener Activate(
            TService service);
    }
}