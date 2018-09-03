using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulService : Microsoft.ServiceFabric.Services.Runtime.StatefulService, IStatefulService
    {
        private class ServiceEvents
        {
            private readonly SemaphoreSlim whenInitialized;

            private readonly SemaphoreSlim whenRoleChanged;

            public ServiceEvents()
            {
                this.whenInitialized = new SemaphoreSlim(0, 1);
                this.whenRoleChanged = new SemaphoreSlim(0, 1);
            }

            public event EventHandler<NotifyAsyncEventArgs> OnStartup;

            public event EventHandler<NotifyAsyncEventArgs> OnChangeRole;

            public event EventHandler<NotifyAsyncEventArgs> OnRun;

            public event EventHandler<NotifyAsyncEventArgs> OnDataLoss;

            public event EventHandler<NotifyAsyncEventArgs> OnRestoreCompleted;

            public event EventHandler<NotifyAsyncEventArgs<IStatefulServiceEventPayloadShutdown>> OnShutdown;

            public ListenerEvents CreateListenerEvents()
            {
                return new ListenerEvents(this.whenInitialized);
            }

            public async Task SynchronizeWhenInitializedAsync()
            {
                await this.whenInitialized.WaitAsync();
            }

            public async Task SynchronizeWhenRoleChangedAsync()
            {
                await this.whenRoleChanged.WaitAsync();
            }

            public Task NotifyStartupAsync(
                CancellationToken cancellationToken)
            {
                return this.OnStartup.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyChangeRoleAsync(
                CancellationToken cancellationToken)
            {
                return this.OnChangeRole.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyRunAsync(
                CancellationToken cancellationToken)
            {
                return this.OnRun.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyShutdownAsync(
                IStatefulServiceEventPayloadShutdown payload,
                CancellationToken cancellationToken)
            {
                return this.OnShutdown.NotifyAsync(this, payload, cancellationToken);
            }

            public Task NotifyDataLossAsync(
                CancellationToken cancellationToken)
            {
                return this.OnDataLoss.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyRestoreCompletedAsync(
                CancellationToken cancellationToken)
            {
                return this.OnRestoreCompleted.NotifyAsync(this, cancellationToken);
            }

            public void SignalInitialized()
            {
                this.whenInitialized.Release();
            }

            public void SignalRoleChanged()
            {
                this.whenRoleChanged.Release();
            }
        }

        private class ListenerEvents
        {
            private readonly SemaphoreSlim whenServiceInitialized;

            public ListenerEvents(
                SemaphoreSlim whenServiceInitialized)
            {
                this.whenServiceInitialized = whenServiceInitialized
                 ?? throw new ArgumentNullException(nameof(whenServiceInitialized));
            }

            public event EventHandler<NotifyAsyncEventArgs> Opened;

            public event EventHandler<NotifyAsyncEventArgs> Closed;

            public event EventHandler<NotifyAsyncEventArgs> Aborted;

            public async Task SynchronizeWhenServiceInitializedAsync()
            {
                await this.whenServiceInitialized.WaitAsync();

                this.whenServiceInitialized.Release();
            }

            public async Task NotifyOpenedAsync(
                CancellationToken cancellationToken)
            {
                await this.Opened.NotifyAsync(this, cancellationToken);
            }

            public async Task NotifyClosedAsync(
                CancellationToken cancellationToken)
            {
                await this.Closed.NotifyAsync(this, cancellationToken);
            }

            public void NotifyAborted(
                CancellationToken cancellationToken)
            {
                this.Aborted.NotifyAsync(this, cancellationToken).GetAwaiter().GetResult();
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
                    await this.events.SynchronizeWhenServiceInitializedAsync();

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

        private readonly ILookup<StatefulServiceLifecycleEvent, StatefulServiceDelegate> serviceDelegates;

        private readonly IReadOnlyList<ServiceReplicaListener> serviceListeners;

        public StatefulService(
            StatefulServiceContext serviceContext,
            IReadOnlyList<IStatefulServiceHostDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators)
            : base(serviceContext)
        {
            this.eventSource = new ServiceEventSource(
                serviceContext,
                $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}",
                EventSourceSettings.EtwSelfDescribingEventFormat);

            this.serviceEvents = new ServiceEvents();

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
                            var events = this.serviceEvents.CreateListenerEvents();
                            var replicaListener = replicator.ReplicateFor(this);
                            return new ServiceReplicaListener(
                                context => new ListenerEventsDecorator(events, replicaListener.CreateCommunicationListener(context)),
                                replicaListener.Name);
                        })
                   .ToList();
            }

            this.serviceEvents.OnStartup += async (
                sender,
                args) =>
            {
                try
                {
                    this.eventSource.Information<ServiceEventSourceData>(-1,"OnStartup","StatefulEvents","OnStartup", new ServiceEventSourceData());

                    var context = new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnStartup);

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
                    this.eventSource.Information<ServiceEventSourceData>(-1,"OnRun","StatefulEvents","OnRun", new ServiceEventSourceData());

                    var context = new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnRun);

                    await this.InvokeDelegates(context, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
            this.serviceEvents.OnChangeRole += async (
                sender,
                args) =>
            {
                try
                {
                    this.eventSource.Information<ServiceEventSourceData>(-1,"OnChangeRole","StatefulEvents","OnChangeRole", new ServiceEventSourceData());

                    var context = new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnChangeRole);

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
                    this.eventSource.Information<ServiceEventSourceData>(-1,"OnShutdown","StatefulEvents","OnShutdown", new ServiceEventSourceData());

                    var context = new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnShutdown);

                    await this.InvokeDelegates(context, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
            this.serviceEvents.OnDataLoss += async (
                sender,
                args) =>
            {
                try
                {
                    this.eventSource.Information<ServiceEventSourceData>(-1,"OnDataLoss","StatefulEvents","OnDataLoss", new ServiceEventSourceData());

                    var context = new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnDataLoss);

                    await this.InvokeDelegates(context, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
            this.serviceEvents.OnRestoreCompleted += async (
                sender,
                args) =>
            {
                try
                {
                    this.eventSource.Information<ServiceEventSourceData>(-1,"OnRestoreCompleted","StatefulEvents","OnRestoreCompleted", new ServiceEventSourceData());

                    var context = new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnRestoreCompleted);

                    await this.InvokeDelegates(context, args.CancellationToken);

                    args.Completed();
                }
                catch (Exception e)
                {
                    args.Failed(e);
                }
            };
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.GetServiceListeners();
        }

        protected override async Task OnChangeRoleAsync(
            ReplicaRole newRole,
            CancellationToken cancellationToken)
        {
            if (newRole == ReplicaRole.Primary)
            {
                await this.serviceEvents.SynchronizeWhenInitializedAsync();
            }

            await this.serviceEvents.NotifyChangeRoleAsync(cancellationToken);

            if (newRole == ReplicaRole.Primary)
            {
                this.serviceEvents.SignalRoleChanged();
            }
        }

        protected override async Task RunAsync(
            CancellationToken cancellationToken)
        {
            await this.serviceEvents.NotifyStartupAsync(cancellationToken);

            this.serviceEvents.SignalInitialized();

            await this.serviceEvents.SynchronizeWhenRoleChangedAsync();

            await this.serviceEvents.NotifyRunAsync(cancellationToken);
        }

        protected override async Task OnCloseAsync(
            CancellationToken cancellationToken)
        {
            var payload = new StatefulServiceEventPayloadShutdown(false);

            await this.serviceEvents.NotifyShutdownAsync(payload, cancellationToken);
        }

        protected override void OnAbort()
        {
            var payload = new StatefulServiceEventPayloadShutdown(false);

            this.serviceEvents.NotifyShutdownAsync(payload, default).GetAwaiter().GetResult();
        }

        protected override async Task<bool> OnDataLossAsync(
            RestoreContext restoreCtx,
            CancellationToken cancellationToken)
        {
            await this.serviceEvents.NotifyDataLossAsync(cancellationToken);

            return false;
        }

        protected override async Task OnRestoreCompletedAsync(
            CancellationToken cancellationToken)
        {
            await this.serviceEvents.NotifyRestoreCompletedAsync(cancellationToken);
        }

        public IReliableStateManager GetReliableStateManager()
        {
            return this.StateManager;
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
            IStatefulServiceDelegateInvocationContext context,
            CancellationToken cancellationToken = default)
        {
            var delegates = this.GetServiceDelegates(context.Event);
            foreach (var @delegate in delegates)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var invoker = @delegate.CreateDelegateInvokerFunc();

                await invoker.InvokeAsync(context, cancellationToken);
            }
        }

        private IEnumerable<StatefulServiceDelegate> GetServiceDelegates(
            StatefulServiceLifecycleEvent @event)
        {
            return this.serviceDelegates == null
                ? Enumerable.Empty<StatefulServiceDelegate>()
                : this.serviceDelegates[@event];
        }

        private IEnumerable<ServiceReplicaListener> GetServiceListeners()
        {
            return this.serviceListeners ?? Enumerable.Empty<ServiceReplicaListener>();
        }
    }
}