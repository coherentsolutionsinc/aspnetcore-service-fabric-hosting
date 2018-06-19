using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessService : Microsoft.ServiceFabric.Services.Runtime.StatelessService, IStatelessService
    {
        private class EventSynchronization
        {
            private readonly TaskCompletionSource<int> whenAllListenersOpenedTaskSource;

            private SpinLock spinLock;

            private int remainingListenersCount;

            public EventSynchronization(
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
                try
                {
                    this.spinLock.Enter(ref lockTaken);

                    if (--this.remainingListenersCount == 0)
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

            public async Task WhenAllListenersOpened(
                CancellationToken cancellationToken)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                using (cancellationToken.Register(
                    () =>
                    {
                        var lockTaken = false;
                        try
                        {
                            this.spinLock.Enter(ref lockTaken);

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

        private class ListenerEventDecorator : ICommunicationListener
        {
            private readonly ICommunicationListener successor;

            private readonly EventSynchronization eventSynchronization;

            public ListenerEventDecorator(
                EventSynchronization eventSynchronization,
                ICommunicationListener successor)
            {
                this.eventSynchronization = eventSynchronization
                 ?? throw new ArgumentNullException(nameof(eventSynchronization));

                this.successor = successor
                 ?? throw new ArgumentNullException(nameof(successor));
            }

            public async Task<string> OpenAsync(
                CancellationToken cancellationToken)
            {
                try
                {
                    return await this.successor.OpenAsync(cancellationToken);
                }
                finally
                {
                    this.eventSynchronization.NotifyListenerOpened();
                }
            }

            public Task CloseAsync(
                CancellationToken cancellationToken)
            {
                return this.successor.CloseAsync(cancellationToken);
            }

            public void Abort()
            {
                this.successor.Abort();
            }
        }

        private readonly IReadOnlyList<IStatelessServiceHostAsyncDelegateReplicator> serviceDelegateReplicators;

        private readonly IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators;

        private readonly ServiceEventSource eventSource;

        private readonly EventSynchronization eventSynchronization;

        public StatelessService(
            StatelessServiceContext serviceContext,
            IReadOnlyList<IStatelessServiceHostAsyncDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators)
            : base(serviceContext)
        {
            if (serviceListenerReplicators == null)
            {
                throw new ArgumentNullException(nameof(serviceListenerReplicators));
            }

            this.eventSource = new ServiceEventSource(
                serviceContext,
                $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}",
                EventSourceSettings.EtwSelfDescribingEventFormat);

            this.eventSynchronization = new EventSynchronization(
                serviceListenerReplicators.Count);

            this.serviceDelegateReplicators = serviceDelegateReplicators 
             ?? throw new ArgumentNullException(nameof(serviceDelegateReplicators));

            this.serviceListenerReplicators = serviceListenerReplicators;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.serviceListenerReplicators
               .Select(
                    replicator =>
                    {
                        var replicaListener = replicator.ReplicateFor(this);
                        return new ServiceInstanceListener(
                            context =>
                            {
                                return new ListenerEventDecorator(
                                    this.eventSynchronization,
                                    replicaListener.CreateCommunicationListener(context));
                            },
                            replicaListener.Name);
                    });
        }

        protected override async Task RunAsync(
            CancellationToken cancellationToken)
        {
            // Wait when all listeners are opened
            await this.eventSynchronization.WhenAllListenersOpened(cancellationToken);

            // Run async operations
        }

        public ServiceContext GetContext()
        {
            return this.Context;
        }

        public IServiceEventSource GetEventSource()
        {
            return this.eventSource;
        }

        public IServicePartition GetPartition()
        {
            return this.Partition;
        }
    }
}