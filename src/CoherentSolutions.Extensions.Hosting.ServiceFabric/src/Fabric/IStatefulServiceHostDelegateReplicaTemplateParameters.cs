namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostDelegateReplicaTemplateParameters
        : IServiceHostDelegateReplicaTemplateParameters
    {
        StatefulServiceLifecycleEvent Event { get; }
    }
}