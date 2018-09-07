namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostDelegateReplicator
        : IServiceHostDelegateReplicator<IStatelessService, StatelessServiceDelegate>
    {
    }
}