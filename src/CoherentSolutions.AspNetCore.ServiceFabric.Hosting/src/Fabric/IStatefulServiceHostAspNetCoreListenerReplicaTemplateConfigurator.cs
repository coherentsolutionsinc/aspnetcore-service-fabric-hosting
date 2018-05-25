namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        : IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
        void UseListenerOnSecondary();
    }
}