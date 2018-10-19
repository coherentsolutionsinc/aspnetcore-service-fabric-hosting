using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceInformation
    {
        ServiceContext GetContext();
        IServicePartition GetPartition();

    }
}