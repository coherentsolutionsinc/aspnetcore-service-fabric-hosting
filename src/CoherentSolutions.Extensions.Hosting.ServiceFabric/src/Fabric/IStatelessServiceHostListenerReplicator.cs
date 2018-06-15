using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostListenerReplicator
        : IServiceHostListenerReplicator<IStatelessService, ServiceInstanceListener>
    {
    }
}