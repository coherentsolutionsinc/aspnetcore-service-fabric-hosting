using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostListenerReplicableTemplate
        : IServiceHostListenerReplicableTemplate<IStatelessService, ServiceInstanceListener>
    {
    }
}