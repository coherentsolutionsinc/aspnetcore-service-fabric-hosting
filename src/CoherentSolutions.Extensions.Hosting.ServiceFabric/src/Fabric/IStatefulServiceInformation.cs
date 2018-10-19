using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceInformation : IServiceInformation
    {
        IReliableStateManager GetReliableStateManager();
    }
}