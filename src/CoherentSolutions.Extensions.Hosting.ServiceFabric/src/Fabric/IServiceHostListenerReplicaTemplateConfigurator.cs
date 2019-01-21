namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicaTemplateConfigurator
    {
        void UseEndpoint(
            string endpointName);
    }
}