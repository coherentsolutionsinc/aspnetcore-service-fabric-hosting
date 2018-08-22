namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostDelegateReplicableTemplate
        : IServiceHostDelegateReplicableTemplate<IStatelessService, StatelessServiceDelegate>
    {
    }
}