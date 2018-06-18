using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceEventSynchronization
    {
        private readonly TaskCompletionSource<int> whenListenersOpenedTask;

        private int remainingListenersCount;

        public ServiceEventSynchronization(
            int listenersCount)
        {
            if (listenersCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(listenersCount));
            }

            this.remainingListenersCount = listenersCount;
            this.whenListenersOpenedTask = new TaskCompletionSource<int>();
        }

        public void NotifyListenerOpened()
        {
            if (Interlocked.Decrement(ref this.remainingListenersCount) == 0)
            {
                this.whenListenersOpenedTask.SetResult(0);
            }
        }

        public Task WhenListenersOpened()
        {
            return this.whenListenersOpenedTask.Task;
        }
    }
}