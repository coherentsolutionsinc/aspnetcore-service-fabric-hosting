using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceEventSynchronization
        : IServiceEventSynchronization
    {
        private readonly TaskCompletionSource<int> whenAllListenersOpenedTaskSource;

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

            this.whenAllListenersOpenedTaskSource = new TaskCompletionSource<int>();

            if (listenersCount == 0)
            {
                this.whenAllListenersOpenedTaskSource.SetResult(0);
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
                    this.whenAllListenersOpenedTaskSource.SetResult(0);
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

        public async Task WhenAllListenersOpened(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            using (cancellationToken.Register(
                () =>
                {
                    var lockTaken = false;

                    this.spinLock.Enter(ref lockTaken);
                    try
                    {
                        if (this.remainingListenersCount == 0)
                        {
                            return;
                        }

                        this.remainingListenersCount = -1;
                        this.whenAllListenersOpenedTaskSource.SetCanceled();
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            this.spinLock.Exit(true);
                        }
                    }
                }))
            {
                await this.whenAllListenersOpenedTaskSource.Task;
            }
        }
    }
}