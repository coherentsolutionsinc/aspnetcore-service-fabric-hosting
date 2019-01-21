using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostGenericListenerReplicaTemplateConfigurator
        : IServiceHostListenerReplicaTemplateConfigurator,
          IConfigurableObjectDependenciesConfigurator,
          IConfigurableObjectLoggerConfigurator
    {
        void UseCommunicationListener(
            ServiceHostGenericCommunicationListenerFactory factoryFunc);
    }
}