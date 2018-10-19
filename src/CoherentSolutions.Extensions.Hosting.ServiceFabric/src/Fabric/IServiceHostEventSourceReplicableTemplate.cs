using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicableTemplate<out TEventSource>
        where TEventSource : IServiceEventSource
    {
        TEventSource Activate(
            ServiceContext serviceContext);
    }
}