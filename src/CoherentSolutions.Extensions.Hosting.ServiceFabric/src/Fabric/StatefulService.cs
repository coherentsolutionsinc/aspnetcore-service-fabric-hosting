using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;

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
            public ServiceEventBridgeCodePackage CodePackage { get; }

            public ServiceEvents(
                ServiceEventBridgeCodePackage eventBridgeCodePackage)
            {
                this.CodePackage = eventBridgeCodePackage ?? throw new ArgumentNullException(nameof(eventBridgeCodePackage));
            }

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
            this.serviceEvents = new ServiceEvents(
                new ServiceEventBridgeCodePackage(serviceContext.CodePackageActivationContext));

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
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnStartup),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.OnRun += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnRun),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.OnChangeRole += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnChangeRole(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.OnShutdown += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnShutdown(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.OnDataLoss += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnDataLoss(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.OnRestoreCompleted += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnRestoreCompleted),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.CodePackage.OnCodePackageAdded += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnCodePackageAdded(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.CodePackage.OnCodePackageModified += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnCodePackageModified(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.CodePackage.OnCodePackageRemoved += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnCodePackageRemoved(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.CodePackage.OnConfigPackageAdded += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnConfigPackageAdded(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.CodePackage.OnConfigPackageModified += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnConfigPackageModified(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.CodePackage.OnConfigPackageRemoved += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnConfigPackageRemoved(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.CodePackage.OnDataPackageAdded += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnDataPackageAdded(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.CodePackage.OnDataPackageModified += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnDataPackageModified(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
            this.serviceEvents.CodePackage.OnDataPackageRemoved += async (
                sender,
                args) =>
            {
                try
                {
                    await this.InvokeDelegates(
                        new StatefulServiceDelegateInvocationContextOnDataPackageRemoved(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
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

                var invoker = @delegate.CreateDelegateInvoker();

                await invoker.InvokeAsync(@delegate.Delegate, context, cancellationToken);
            }
        }

        private IEnumerable<StatefulServiceDelegate> GetServiceDelegates(
            StatefulServiceLifecycleEvent @event)
        {
            return this.serviceDelegates == null
                ? Array.Empty<StatefulServiceDelegate>()
                : this.serviceDelegates[@event];
        }

        private IEnumerable<ServiceReplicaListener> GetServiceListeners()
        {
            return this.serviceListeners ?? Array.Empty<ServiceReplicaListener>();
        }

        private StatefulServiceEventSource GetServiceEventSource()
        {
            return this.serviceEventSource;
        }
    }
}