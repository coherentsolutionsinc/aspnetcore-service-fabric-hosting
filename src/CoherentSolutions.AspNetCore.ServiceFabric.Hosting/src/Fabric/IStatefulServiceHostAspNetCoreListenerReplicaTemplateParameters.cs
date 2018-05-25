namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters
        : IServiceHostAspNetCoreListenerReplicaTemplateParameters
    {
        bool ListenerOnSecondary { get; }
    }
}