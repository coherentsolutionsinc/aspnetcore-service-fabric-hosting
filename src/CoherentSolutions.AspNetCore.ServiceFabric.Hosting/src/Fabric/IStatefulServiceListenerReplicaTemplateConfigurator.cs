namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceListenerReplicaTemplateConfigurator
    {
        void UseListenerOnSecondary();
    }
}