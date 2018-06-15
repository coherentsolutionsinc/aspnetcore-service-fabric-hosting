using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IService
    {
        ServiceContext GetContext();

        IServiceEventSource GetEventSource();

        IServicePartition GetPartition();
    }
}