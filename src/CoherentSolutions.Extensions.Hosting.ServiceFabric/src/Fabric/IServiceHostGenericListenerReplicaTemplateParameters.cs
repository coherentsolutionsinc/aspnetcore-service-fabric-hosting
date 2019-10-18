using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostGenericListenerReplicaTemplateParameters
        : IServiceHostListenerReplicaTemplateParameters,
          IConfigurableObjectDependenciesParameters
    {
        ServiceHostGenericCommunicationListenerFactory GenericCommunicationListenerFunc { get; }
    }
}