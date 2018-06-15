using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHost
    {
        Task StartAsync(
            CancellationToken cancellationToken);

        Task StopAsync(
            CancellationToken cancellationToken);
    }
}