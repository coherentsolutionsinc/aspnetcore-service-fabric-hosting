using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostGenericListenerReplicaTemplateConfigurator
        : IServiceHostListenerReplicaTemplateConfigurator,
          IConfigurableObjectDependenciesConfigurator
    {
        void UseCommunicationListener(
            ServiceHostGenericCommunicationListenerFactory factoryFunc);
    }
}