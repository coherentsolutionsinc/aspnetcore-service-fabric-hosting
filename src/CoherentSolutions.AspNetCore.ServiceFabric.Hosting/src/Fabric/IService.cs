using System.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IService
    {
        ServiceContext GetContext();

        IServiceEventSource GetEventSource();

        IServicePartition GetPartition();
    }
}