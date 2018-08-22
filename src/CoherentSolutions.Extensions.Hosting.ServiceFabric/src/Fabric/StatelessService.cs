using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

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

        private readonly ServiceEventSource eventSource;

        private readonly EventSynchronization eventSynchronization;

        private readonly ILookup<StatelessServiceLifecycleEvent, StatelessServiceDelegate> serviceDelegates;

        private readonly IReadOnlyList<ServiceInstanceListener> serviceListeners;

        public StatelessService(
            StatelessServiceContext serviceContext,
            IReadOnlyList<IStatelessServiceHostDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators)
            : base(serviceContext)
        {
            this.eventSource = new ServiceEventSource(
                serviceContext,
                $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}",
                EventSourceSettings.EtwSelfDescribingEventFormat);

            this.eventSynchronization = new EventSynchronization(
                serviceListenerReplicators.Count);

            if (serviceDelegateReplicators != null)
            {
                this.serviceDelegates = serviceDelegateReplicators
                   .SelectMany(
                        replicator =>
                        {
                            var @delegate = replicator.ReplicateFor(this);
                            return @delegate.Event.GetBitFlags().Select(v => (v, @delegate));
                        })
                   .ToLookup(kv => kv.v, kv => kv.@delegate);
            }

            if (serviceListenerReplicators != null)
            {
                this.serviceListeners = serviceListenerReplicators
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
                        })
                   .ToList();
            }
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.GetServiceListeners();
        }

        protected override async Task RunAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.eventSynchronization.WhenAllListenersOpened(cancellationToken);

            await this.InvokeDelegates(StatelessServiceLifecycleEvent.OnRunAfterListenersAreOpened, cancellationToken);
        }

        protected override async Task OnOpenAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.InvokeDelegates(StatelessServiceLifecycleEvent.OnOpen, cancellationToken);
        }

        protected override async Task OnCloseAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.InvokeDelegates(StatelessServiceLifecycleEvent.OnClose, cancellationToken);
        }

        protected override void OnAbort()
        {
            Task.Run(async () => await this.InvokeDelegates(StatelessServiceLifecycleEvent.OnAbort)).Wait();
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

        private async Task InvokeDelegates(
            StatelessServiceLifecycleEvent @event,
            CancellationToken cancellationToken = default)
        {
            var context = new StatelessServiceDelegateInvocationContext(@event);
            var delegates = this.GetServiceDelegates(@event);
            foreach (var @delegate in delegates)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var invoker = @delegate.CreateDelegateInvokerFunc();

                await invoker.InvokeAsync(context, cancellationToken);
            }
        }

        private IEnumerable<StatelessServiceDelegate> GetServiceDelegates(
            StatelessServiceLifecycleEvent @event)
        {
            return this.serviceDelegates == null
                ? Enumerable.Empty<StatelessServiceDelegate>()
                : this.serviceDelegates[@event];
        }

        private IEnumerable<ServiceInstanceListener> GetServiceListeners()
        {
            return this.serviceListeners ?? Enumerable.Empty<ServiceInstanceListener>();
        }
    }
}