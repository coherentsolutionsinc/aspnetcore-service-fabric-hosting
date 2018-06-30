namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicaTemplateConfigurator : IServiceHostLoggerConfigurator
    {
        void UseEndpoint(
            string endpointName);
    }
}