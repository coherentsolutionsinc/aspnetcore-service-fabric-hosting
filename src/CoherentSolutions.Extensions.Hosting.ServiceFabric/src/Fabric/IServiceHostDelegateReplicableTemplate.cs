namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateReplicableTemplate<in TService, out TDelegate>
    {
        TDelegate Activate(
            TService service);
    }
}