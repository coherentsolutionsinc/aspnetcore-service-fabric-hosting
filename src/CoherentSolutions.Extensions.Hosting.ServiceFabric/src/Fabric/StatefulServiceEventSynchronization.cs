using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceEventSynchronization : IServiceEventSynchronization
    {
        public void NotifyListenerOpened()
        {
        }

        public Task WhenAllListenersOpened(
            CancellationToken cancellationToken)
        {
            return new TaskCompletionSource<int>().Task;
        }
    }
}