namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicaTemplateConfigurator : IServiceHostLoggerConfigurator
    {
        void UseEndpointName(
            string endpointName);
    }
}