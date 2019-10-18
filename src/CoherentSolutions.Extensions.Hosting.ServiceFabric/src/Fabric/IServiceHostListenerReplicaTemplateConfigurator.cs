using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicaTemplateConfigurator : 
        IConfigurableObjectLoggerConfigurator
    {
        void UseEndpoint(
            string endpointName);
    }
}