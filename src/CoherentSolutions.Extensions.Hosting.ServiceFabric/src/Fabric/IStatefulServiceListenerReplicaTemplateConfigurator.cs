namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceListenerReplicaTemplateConfigurator
    {
        void UseListenerOnSecondary();
    }
}