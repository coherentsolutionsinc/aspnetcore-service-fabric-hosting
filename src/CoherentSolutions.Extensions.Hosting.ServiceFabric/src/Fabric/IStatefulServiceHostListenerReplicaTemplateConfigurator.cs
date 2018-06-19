namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostListenerReplicaTemplateConfigurator
    {
        void UseListenerOnSecondary();
    }
}