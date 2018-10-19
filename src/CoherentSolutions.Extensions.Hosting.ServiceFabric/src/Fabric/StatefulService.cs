using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulService
        : Microsoft.ServiceFabric.Services.Runtime.StatefulService,
          IStatefulService,
          IStatefulServiceInformation
    {
        private class ServiceEvents
        {
            public event EventHandler<NotifyAsyncEventArgs> OnStartup;

            public event EventHandler<NotifyAsyncEventArgs<IStatefulServiceEventPayloadOnChangeRole>> OnChangeRole;

            public event EventHandler<NotifyAsyncEventArgs> OnRun;

            public event EventHandler<NotifyAsyncEventArgs<IStatefulServiceEventPayloadOnShutdown>> OnShutdown;

            public event EventHandler<NotifyAsyncEventArgs<IStatefulServiceEventPayloadOnDataLoss>> OnDataLoss;

            public event EventHandler<NotifyAsyncEventArgs> OnRestoreCompleted;

            public Task NotifyStartupAsync(
                CancellationToken cancellationToken)
            {
                return this.OnStartup.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyChangeRoleAsync(
                IStatefulServiceEventPayloadOnChangeRole payload,
                CancellationToken cancellationToken)
            {
                return this.OnChangeRole.NotifyAsync(this, payload, cancellationToken);
            }

            public Task NotifyRunAsync(
                CancellationToken cancellationToken)
            {
                return this.OnRun.NotifyAsync(this, cancellationToken);
            }

            public Task NotifyShutdownAsync(
                IStatefulServiceEventPayloadOnShutdown payload,
                CancellationToken cancellationToken)
            {
                return this.OnShutdown.NotifyAsync(this, payload, cancellationToken);
            }

            public Task NotifyDataLossAsync(
                IStatefulServiceEventPayloadOnDataLoss payload,
                CancellationToken cancellationToken)
            {
                return this.OnDataLoss.NotifyAsync(this, payload, cancellationToken);
            }

            public Task NotifyRestoreCompletedAsync(
                CancellationToken cancellationToken)
            {
                return this.OnRestoreCompleted.NotifyAsync(this, cancellationToken);
            }
        }

        private readonly ServiceEvents serviceEvents;

        private readonly StatefulServiceEventSource serviceEventSource;

        private readonly ILookup<StatefulServiceLifecycleEvent, StatefulServiceDelegate> serviceDelegates;

        private readonly IReadOnlyList<ServiceReplicaListener> serviceListeners;

        public StatefulService(
            StatefulServiceContext serviceContext,
            IStatefulServiceHostEventSourceReplicator serviceEventSourceReplicator,
            IReadOnlyList<IStatefulServiceHostDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators)
            : base(serviceContext)
        {
            this.serviceEvents = new ServiceEvents();

            this.serviceEventSource = serviceEventSourceReplicator.ReplicateFor(this);
            if (this.serviceEventSource == null)
            {
                throw new ReplicatorProducesNullInstanceException<StatefulServiceEventSource>();
            }

            if (serviceDelegateReplicators != null)
            {
                this.serviceDelegates = serviceDelegateReplicators
                   .SelectMany(
                        replicator =>
                        {
                            var @delegate = replicator.ReplicateFor(this);
                            if (@delegate == null)
                            {
                                throw new ReplicatorProducesNullInstanceException<StatefulServiceDelegate>();
                            }

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
                            var listener = replicator.ReplicateFor(this);
                            if (listener == null)
                            {
                                throw new ReplicatorProducesNullInstanceException<ServiceReplicaListener>();
                            }

                            return listener;
                        })
                   .ToList();
            }

            this.serviceEvents.OnStartup += async (
                sender,
                args) =>
            {
                try
                {
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
                    var context = new StatefulServiceDelegateInvocationContextOnChangeRole(args.Payload);

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
                    var context = new StatefulServiceDelegateInvocationContextOnShutdown(args.Payload);

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
                    var context = new StatefulServiceDelegateInvocationContextOnDataLoss(args.Payload);

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

        protected override async Task OnOpenAsync(
            ReplicaOpenMode openMode,
            CancellationToken cancellationToken)
        {
            await this.serviceEvents.NotifyStartupAsync(cancellationToken);
        }

        protected override async Task OnChangeRoleAsync(
            ReplicaRole newRole,
            CancellationToken cancellationToken)
        {
            var payload = new StatefulServiceEventPayloadOnChangeRole(newRole);

            await this.serviceEvents.NotifyChangeRoleAsync(payload, cancellationToken);
        }

        protected override async Task RunAsync(
            CancellationToken cancellationToken)
        {
            await this.serviceEvents.NotifyRunAsync(cancellationToken);
        }

        protected override async Task OnCloseAsync(
            CancellationToken cancellationToken)
        {
            var payload = new StatefulServiceEventPayloadOnShutdown(false);

            await this.serviceEvents.NotifyShutdownAsync(payload, cancellationToken);
        }

        protected override void OnAbort()
        {
            var payload = new StatefulServiceEventPayloadOnShutdown(false);

            this.serviceEvents.NotifyShutdownAsync(payload, default).GetAwaiter().GetResult();
        }

        protected override async Task<bool> OnDataLossAsync(
            RestoreContext restoreCtx,
            CancellationToken cancellationToken)
        {
            var ctx = new StatefulServiceRestoreContext(restoreCtx);
            var payload = new StatefulServiceEventPayloadOnDataLoss(ctx);

            await this.serviceEvents.NotifyDataLossAsync(payload, cancellationToken);

            return ctx.IsRestored;
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
            return this.CreateEventSource();
        }

        public IServicePartition GetPartition()
        {
            return this.Partition;
        }

        private IServiceEventSource CreateEventSource()
        {
            return this.GetServiceEventSource().CreateEventSourceFunc();
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

        private StatefulServiceEventSource GetServiceEventSource()
        {
            return this.serviceEventSource;
        }
    }
}