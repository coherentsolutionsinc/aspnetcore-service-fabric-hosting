namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostDelegateReplicator
        : IServiceHostDelegateReplicator<IStatefulService, IServiceHostDelegateInvoker>
    {
    }
}