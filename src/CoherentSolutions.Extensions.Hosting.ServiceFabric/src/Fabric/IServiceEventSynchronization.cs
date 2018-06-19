using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceEventSynchronization
    {
        void NotifyListenerOpened();

        Task WhenAllListenersOpened(
            CancellationToken cancellationToken);
    }
}