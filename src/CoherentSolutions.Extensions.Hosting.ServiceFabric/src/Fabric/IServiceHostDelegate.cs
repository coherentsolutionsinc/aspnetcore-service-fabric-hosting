using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegate
    {
        ServiceLifecycleEvent LifecycleEvent { get; }

        Task InvokeAsync(
            CancellationToken cancellationToken);
    }
}