namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostDelegateReplicableTemplate
        : IServiceHostDelegateReplicableTemplate<IStatefulService, StatefulServiceDelegate>
    {
    }
}