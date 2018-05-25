using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatelessServiceHostListenerReplicator
        : IServiceHostListenerReplicator<IStatelessService, ServiceInstanceListener>
    {
    }
}