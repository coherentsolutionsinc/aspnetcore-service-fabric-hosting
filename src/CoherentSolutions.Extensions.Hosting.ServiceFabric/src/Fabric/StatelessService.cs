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
        private class NotificationEventArgs : EventArgs
        {
            private readonly Action completion;

            private readonly Action<Exception> failure;

            public CancellationToken CancellationToken { get; }

            public NotificationEventArgs(
                Action completion,
                Action<Exception> failure,
                CancellationToken cancellationToken)
            {
                this.completion = completion;
                this.failure = failure;
                this.CancellationToken = cancellationToken;
            }

            public void Completed()
            {
                this.completion();
            }

            public void Failed(
                Exception exception)
            {
                this.failure(exception);
            }
        }

        private class ServiceEvents
        {
            private readonly TaskCompletionSource<bool> canOpenListenersTaskCompletionSource;

            private readonly TaskCompletionSource<bool> whenAllListenersOpenedTaskCompletionSource;

            private int listenerCount;

            public event EventHandler<NotificationEventArgs> OnRunBeforeListenersOpened;

            public event EventHandler<NotificationEventArgs> OnRunAfterListenersOpened;

            public event EventHandler<NotificationEventArgs> OnOpen;

            public event EventHandler<NotificationEventArgs> OnClose;

            public event EventHandler<NotificationEventArgs> OnAbort;

            public ServiceEvents(
                int listenerCount = 0)
            {
                this.listenerCount = listenerCount;

                this.whenAllListenersOpenedTaskCompletionSource = new TaskCompletionSource<bool>();
                this.canOpenListenersTaskCompletionSource = new TaskCompletionSource<bool>();

                if (this.listenerCount == 0)
                {
                    this.whenAllListenersOpenedTaskCompletionSource.SetResult(true);
                }
            }

            public ListenerEvents CreateListenerEvents()
            {
                var listenerEvents = new ListenerEvents(this.canOpenListenersTaskCompletionSource.Task);
                listenerEvents.Opened += (
                    sender,
                    args) =>
                {
                    try
                    {
                        if (Interlocked.Decrement(ref this.listenerCount) == 0)
                        {
                            this.whenAllListenersOpenedTaskCompletionSource.SetResult(true);
                        }

                        args.Completed();
                    }
                    catch (Exception e)
                    {
                        args.Failed(e);
                    }
                };

                return listenerEvents;
            }

            public async Task SynchronizeWhenAllListenersOpened(
                CancellationToken cancellationToken)
            {
                using (cancellationToken.Register(
                    () =>
                    {
                        this.whenAllListenersOpenedTaskCompletionSource.SetCanceled();
                    }))
                {
                    await this.whenAllListenersOpenedTaskCompletionSource.Task;
                }
            }

            public async Task NotifyRunBeforeListenersOpened(
                CancellationToken cancellationToken)
            {
                await this.NotifyAsync(this.OnRunBeforeListenersOpened, cancellationToken);

                this.canOpenListenersTaskCompletionSource.SetResult(true);
            }

            public async Task  NotifyRunAfterListenersOpenedAsync(
                CancellationToken cancellationToken)
            {
                await this.NotifyAsync(this.OnRunAfterListenersOpened, cancellationToken);
            }

            public async Task  NotifyOpenAsync(
                CancellationToken cancellationToken)
            {
                await this.NotifyAsync(this.OnOpen, cancellationToken);
            }

            public async Task  NotifyCloseAsync(
                CancellationToken cancellationToken)
            {
                await this.NotifyAsync(this.OnClose, cancellationToken);
            }

            public void  NotifyAbort(
                CancellationToken cancellationToken)
            {
                this.NotifyAsync(this.OnAbort, cancellationToken).GetAwaiter().GetResult();
            }

            private async Task NotifyAsync(
                EventHandler<NotificationEventArgs> @event,
                CancellationToken cancellationToken)
            {
                var handler = @event;
                if (handler == null)
                {
                    return;
                }

                var tcs = new TaskCompletionSource<bool>();

                handler(
                    this,
                    new NotificationEventArgs(
                        () => tcs.SetResult(true),
                        exception => tcs.SetException(exception),
                        cancellationToken));

                await tcs.Task;
            }
        }

        private class ListenerEvents
        {
            private readonly Task canOpenTask;

            public ListenerEvents(
                Task canOpenTask)
            {
                this.canOpenTask = canOpenTask
                 ?? throw new ArgumentNullException(nameof(canOpenTask));
            }

            public event EventHandler<NotificationEventArgs> Opened;

            public event EventHandler<NotificationEventArgs> Closed;

            public event EventHandler<NotificationEventArgs> Aborted;

            public Task SynchronizeWhenCanOpen(
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return this.canOpenTask;
            }

            public async Task NotifyOpenedAsync(
                CancellationToken cancellationToken)
            {
                await this.NotifyAsync(this.Opened, cancellationToken);
            }

            public async Task NotifyClosedAsync(
                CancellationToken cancellationToken)
            {
                await this.NotifyAsync(this.Closed, cancellationToken);
            }

            public void NotifyAborted(
                CancellationToken cancellationToken)
            {
                this.NotifyAsync(this.Aborted, cancellationToken).GetAwaiter().GetResult();
            }

            private async Task NotifyAsync(
                EventHandler<NotificationEventArgs> @event,
                CancellationToken cancellationToken)
            {
                var handler = @event;
                if (handler == null)
                {
                    return;
                }

                var tcs = new TaskCompletionSource<bool>();
                
                handler(
                    this, 
                    new NotificationEventArgs(
                        () => tcs.SetResult(true), 
                        exception => tcs.SetException(exception),
                        cancellationToken));

                await tcs.Task;
            }
        }

        private class ListenerEventsDecorator : ICommunicationListener
        {
            private readonly ListenerEvents events;

            private readonly ICommunicationListener successor;

            public ListenerEventsDecorator(
                ListenerEvents events,
                ICommunicationListener successor)
            {
                this.events = events
                 ?? throw new ArgumentNullException(nameof(events));

                this.successor = successor
                 ?? throw new ArgumentNullException(nameof(successor));
            }

            public async Task<string> OpenAsync(
                CancellationToken cancellationToken)
            {
                try
                {
                    await this.events.SynchronizeWhenCanOpen(cancellationToken);

                    return await this.successor.OpenAsync(cancellationToken);
                }
                finally
                {
                    await this.events.NotifyOpenedAsync(cancellationToken);
                }
            }

            public async Task CloseAsync(
                CancellationToken cancellationToken)
            {
                try
                {
                    await this.successor.CloseAsync(cancellationToken);
                }
                finally
                {
                    await this.events.NotifyClosedAsync(cancellationToken);
                }
            }

            public void Abort()
            {
                try
                {
                    this.successor.Abort();
                }
                finally
                {
                    this.events.NotifyAborted(CancellationToken.None);
                }
            }
        }

        private readonly ServiceEventSource eventSource;

        private readonly ServiceEvents serviceEvents;

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
                this.serviceEvents = new ServiceEvents(serviceListenerReplicators.Count);
                this.serviceListeners = serviceListenerReplicators
                   .Select(
                        replicator =>
                        {
                            var events = this.serviceEvents.CreateListenerEvents();
                            var replicaListener = replicator.ReplicateFor(this);
                            return new ServiceInstanceListener(
                                context => new ListenerEventsDecorator(events, replicaListener.CreateCommunicationListener(context)),
                                replicaListener.Name);
                        })
                   .ToList();
            }
            else
            {
                this.serviceEvents = new ServiceEvents();
            }

            this.serviceEvents.OnRunBeforeListenersOpened += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(StatelessServiceLifecycleEvent.OnRunBeforeListenersOpened, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
            this.serviceEvents.OnRunAfterListenersOpened += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(StatelessServiceLifecycleEvent.OnRunAfterListenersOpened, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
            this.serviceEvents.OnOpen += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(StatelessServiceLifecycleEvent.OnOpen, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
            this.serviceEvents.OnClose += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(StatelessServiceLifecycleEvent.OnClose, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
            this.serviceEvents.OnAbort += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(StatelessServiceLifecycleEvent.OnAbort, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.GetServiceListeners();
        }

        protected override async Task RunAsync(
            CancellationToken cancellationToken)
        {
            await this.serviceEvents.NotifyRunBeforeListenersOpened(cancellationToken);

            await this.serviceEvents.SynchronizeWhenAllListenersOpened(cancellationToken);

            await this.serviceEvents.NotifyRunAfterListenersOpenedAsync(cancellationToken);
        }

        protected override async Task OnOpenAsync(
            CancellationToken cancellationToken)
        {
            await this.serviceEvents.NotifyOpenAsync(cancellationToken);
        }

        protected override async Task OnCloseAsync(
            CancellationToken cancellationToken)
        {
            await this.serviceEvents.NotifyCloseAsync(cancellationToken);
        }

        protected override void OnAbort()
        {
            this.serviceEvents.NotifyAbort(CancellationToken.None);
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
            cancellationToken.ThrowIfCancellationRequested();

            var context = new StatelessServiceDelegateInvocationContext(@event);
            var delegates = this.GetServiceDelegates(@event);
            foreach (var @delegate in delegates)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var invoker = @delegate.CreateDelegateInvokerFunc();

                await invoker.InvokeAsync(context, cancellationToken);
            }
        }

        private IEnumerable<ServiceInstanceListener> GetServiceListeners()
        {
            return this.serviceListeners ?? Enumerable.Empty<ServiceInstanceListener>();
        }

        private IEnumerable<StatelessServiceDelegate> GetServiceDelegates(
            StatelessServiceLifecycleEvent @event)
        {
            return this.serviceDelegates == null
                ? Enumerable.Empty<StatelessServiceDelegate>()
                : this.serviceDelegates[@event];
        }
    }
}