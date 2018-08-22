namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostDelegateReplicaTemplateParameters 
        : IServiceHostDelegateReplicaTemplateParameters
    {
        StatelessServiceLifecycleEvent Event { get; }
    }
}