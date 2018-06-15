using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostListenerReplicator
        : IServiceHostListenerReplicator<IStatefulService, ServiceReplicaListener>
    {
    }
}