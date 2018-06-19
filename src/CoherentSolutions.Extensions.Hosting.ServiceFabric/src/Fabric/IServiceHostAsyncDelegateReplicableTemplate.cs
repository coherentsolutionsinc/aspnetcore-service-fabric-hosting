namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAsyncDelegateReplicableTemplate<in TService, out TDelegate>
    {
        TDelegate Activate(
            TService service);
    }
}