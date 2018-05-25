using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulService : IService
    {
        IReliableStateManager GetReliableStateManager();
    }
}