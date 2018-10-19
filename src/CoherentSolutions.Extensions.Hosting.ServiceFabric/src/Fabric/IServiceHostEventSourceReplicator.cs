using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicator
    {
        IServiceEventSource ReplicateFor(
            ServiceContext serviceContext);
    }
}