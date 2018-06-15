using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulService : IService
    {
        IReliableStateManager GetReliableStateManager();
    }
}