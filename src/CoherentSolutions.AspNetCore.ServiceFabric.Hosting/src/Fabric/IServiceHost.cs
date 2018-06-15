using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHost
    {
        Task StartAsync(
            CancellationToken cancellationToken);

        Task StopAsync(
            CancellationToken cancellationToken);
    }
}