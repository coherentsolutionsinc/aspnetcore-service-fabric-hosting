using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceEventSynchronization
        : IServiceEventSynchronization
    {
        private readonly TaskCompletionSource<int> whenListenersOpenedTask;

        private SpinLock spinLock;

        private int remainingListenersCount;

        public StatelessServiceEventSynchronization(
            int listenersCount)
        {
            if (listenersCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(listenersCount));
            }

            this.spinLock = new SpinLock();
            this.remainingListenersCount = listenersCount;
            this.whenListenersOpenedTask = new TaskCompletionSource<int>();

            if (listenersCount == 0)
            {
                this.whenListenersOpenedTask.SetResult(0);
            }
        }

        public void NotifyListenerOpened()
        {
            var lockTaken = false;

            this.spinLock.Enter(ref lockTaken);
            try
            {
                if (this.remainingListenersCount > 0 && (--this.remainingListenersCount) == 0)
                {
                    this.whenListenersOpenedTask.SetResult(0);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    this.spinLock.Exit(true);
                }
            }
        }

        public Task WhenAllListeners(CancellationToken cancellationToken)
        {
            cancellationToken.Register(
                () =>
                {
                    var lockTaken = false;
                    this.spinLock.Enter(ref lockTaken);
                    try
                    {
                        if (this.remainingListenersCount != 0)
                        {
                            this.remainingListenersCount = -1;
                            this.whenListenersOpenedTask.SetResult(0);
                        }
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            this.spinLock.Exit(true);
                        }
                    }
                });

            return this.whenListenersOpenedTask.Task;
        }
    }
}