using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostListenerReplicableTemplate
        : IServiceHostListenerReplicableTemplate<IStatefulService, ServiceReplicaListener>
    {
    }
}