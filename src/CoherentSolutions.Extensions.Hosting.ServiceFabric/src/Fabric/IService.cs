using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IService
    {
        ServiceContext GetContext();

        IServicePartition GetPartition();

        IServiceEventSource GetEventSource();
    }
}