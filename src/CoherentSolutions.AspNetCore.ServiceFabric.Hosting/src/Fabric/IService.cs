using System.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IService
    {
        IServicePartition GetPartition();
    }
}