using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicaTemplateParameters :
          IServiceHostBuilderBlockParameters
    {
        string EndpointName { get; }
    }
}