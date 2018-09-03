using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessService : Microsoft.ServiceFabric.Services.Runtime.StatelessService, IStatelessService
    {
        private class ServiceEvents
        {
            private readonly TaskCompletionSource<bool> whenStartedTaskCompletionSource;

            private readonly TaskCompletionSource<bool> whenListenersOpenedTaskCompletionSource;

            private int listenerCount;

            public ServiceEvents(
                int listenerCount = 0)
            {
                this.listenerCount = listenerCount;

                this.whenStartedTaskCompletionSource = new TaskCompletionSource<bool>();
                this.whenListenersOpenedTaskCompletionSource = new TaskCompletionSource<bool>();

                if (this.listenerCount == 0)
                {
                    this.whenListenersOpenedTaskCompletionSource.SetResult(true);
                }
            }

            public event EventHandler<NotifyAsyncEventArgs> OnStartup;

            public event EventHandler<NotifyAsyncEventArgs> OnRun;

            public event EventHandler<NotifyAsyncEventArgs<IStatelessServiceEventPayloadShutdown>> OnShutdown;

            public ListenerEvents CreateListenerEvents()
            {
                var listenerEvents = new ListenerEvents(this.whenStartedTaskCompletionSource.Task);
                listenerEvents.Opened += (
                    sender,
                    args) =>
                {
                    try
                    {
                        if (Interlocked.Decrement(ref this.listenerCount) == 0)
                        {
                            this.whenListenersOpenedTaskCompletionSource.SetResult(true);
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

            public Task SynchronizeWhenListenersOpenedAsync()
            {
                return this.whenListenersOpenedTaskCompletionSource.Task;
            }

            public Task NotifyStartupAsync(
                CancellationToken cancellationToken)
            {
                return this.OnStartup.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyRunAsync(
                CancellationToken cancellationToken)
            {
                return this.OnRun.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyShutdownAsync(
                IStatelessServiceEventPayloadShutdown payload,
                CancellationToken cancellationToken)
            {
                return this.OnShutdown.NotifyAsync(this, payload, cancellationToken);
            }

            public void SignalStarted()
            {
                this.whenStartedTaskCompletionSource.SetResult(true);
            }
        }

        private class ListenerEvents
        {
            private readonly Task whenServiceStartedTask;

            public ListenerEvents(
                Task whenServiceStartedTask)
            {
                this.whenServiceStartedTask = whenServiceStartedTask
                 ?? throw new ArgumentNullException(nameof(whenServiceStartedTask));
            }

            public event EventHandler<NotifyAsyncEventArgs> Opened;

            public event EventHandler<NotifyAsyncEventArgs> Closed;

            public event EventHandler<NotifyAsyncEventArgs> Aborted;

            public Task SynchronizeWhenServiceStartedAsync()
            {
                return this.whenServiceStartedTask;
            }

            public Task NotifyOpenedAsync(
                CancellationToken cancellationToken)
            {
                return this.Opened.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyClosedAsync(
                CancellationToken cancellationToken)
            {
                return this.Closed.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyAbortedAsync(
                CancellationToken cancellationToken)
            {
                return this.Aborted.NotifyAsync(this, cancellationToken);
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
                    await this.events.SynchronizeWhenServiceStartedAsync();

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
                    this.events.NotifyAbortedAsync(CancellationToken.None);
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

            this.serviceEvents.OnStartup += async (
                sender,
                args) =>
            {
                try
                {
                    this.eventSource.Information<ServiceEventSourceData>(-1,"OnStartup","StatelessEvents","OnStartup", new ServiceEventSourceData());

                    var context = new StatelessServiceDelegateInvocationContext(StatelessServiceLifecycleEvent.OnStartup);

                    await this.InvokeDelegates(context, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
            this.serviceEvents.OnRun += async (
                sender,
                args) =>
            {
                try
                {
                    this.eventSource.Information<ServiceEventSourceData>(-1,"OnRun","StatelessEvents","OnRun", new ServiceEventSourceData());

                    var context = new StatelessServiceDelegateInvocationContext(StatelessServiceLifecycleEvent.OnRun);

                    await this.InvokeDelegates(context, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
            this.serviceEvents.OnShutdown += async (
                sender,
                args) =>
            {
                try
                {
                    this.eventSource.Information<ServiceEventSourceData>(-1,"OnShutdown","StatelessEvents","OnShutdown", new ServiceEventSourceData());

                    var context = new StatelessServiceDelegateInvocationContext(StatelessServiceLifecycleEvent.OnShutdown);

                    await this.InvokeDelegates(context, args.CancellationToken);

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
            await this.serviceEvents.NotifyStartupAsync(cancellationToken);

            this.serviceEvents.SignalStarted();

            await this.serviceEvents.SynchronizeWhenListenersOpenedAsync();

            await this.serviceEvents.NotifyRunAsync(cancellationToken);
        }

        protected override async Task OnCloseAsync(
            CancellationToken cancellationToken)
        {
            var payload = new StatelessServiceEventPayloadShutdown(false);

            await this.serviceEvents.NotifyShutdownAsync(payload, cancellationToken);
        }

        protected override void OnAbort()
        {
            var payload = new StatelessServiceEventPayloadShutdown(true);

            this.serviceEvents.NotifyShutdownAsync(payload, default).GetAwaiter().GetResult();
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
            IStatelessServiceDelegateInvocationContext context,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var delegates = this.GetServiceDelegates(context.Event);
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