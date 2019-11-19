using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicaTemplateParameters :
          IServiceHostReplicaTemplateParameters
    {
        string EndpointName { get; }
    }
}