using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessService
        : Microsoft.ServiceFabric.Services.Runtime.StatelessService,
          IStatelessService,
          IStatelessServiceInformation
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

            public event EventHandler<NotifyAsyncEventArgs> OnRun;

            public event EventHandler<NotifyAsyncEventArgs<IStatelessServiceEventPayloadOnShutdown>> OnShutdown;

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
                IStatelessServiceEventPayloadOnShutdown payload,
                CancellationToken cancellationToken)
            {
                return this.OnShutdown.NotifyAsync(this, payload, cancellationToken);
            }
        }

        private readonly ServiceEvents serviceEvents;

        private readonly StatelessServiceEventSource serviceEventSource;

        private readonly ILookup<StatelessServiceLifecycleEvent, StatelessServiceDelegate> serviceDelegates;

        private readonly IReadOnlyList<ServiceInstanceListener> serviceListeners;

        public StatelessService(
            StatelessServiceContext serviceContext,
            IStatelessServiceHostEventSourceReplicator serviceEventSourceReplicator,
            IReadOnlyList<IStatelessServiceHostDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators)
            : base(serviceContext)
        {
            this.serviceEvents = new ServiceEvents(
                new ServiceEventBridgeCodePackage(serviceContext.CodePackageActivationContext));

            this.serviceEventSource = serviceEventSourceReplicator.ReplicateFor(this);
            if (this.serviceEventSource == null)
            {
                throw new ReplicatorProducesNullInstanceException<StatelessServiceEventSource>();
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
                                throw new ReplicatorProducesNullInstanceException<StatelessServiceDelegate>();
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
                                throw new ReplicatorProducesNullInstanceException<ServiceInstanceListener>();
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
                        new StatelessServiceDelegateInvocationContext(StatelessServiceLifecycleEvent.OnStartup),
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
                        new StatelessServiceDelegateInvocationContext(StatelessServiceLifecycleEvent.OnRun),
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
                        new StatelessServiceDelegateInvocationContextOnShutdown(args.Payload),
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
                        new StatelessServiceDelegateInvocationContextOnCodePackageAdded(args.Payload),
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
                        new StatelessServiceDelegateInvocationContextOnCodePackageModified(args.Payload),
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
                        new StatelessServiceDelegateInvocationContextOnCodePackageRemoved(args.Payload),
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
                        new StatelessServiceDelegateInvocationContextOnConfigPackageAdded(args.Payload),
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
                        new StatelessServiceDelegateInvocationContextOnConfigPackageModified(args.Payload),
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
                        new StatelessServiceDelegateInvocationContextOnConfigPackageRemoved(args.Payload),
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
                        new StatelessServiceDelegateInvocationContextOnDataPackageAdded(args.Payload),
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
                        new StatelessServiceDelegateInvocationContextOnDataPackageModified(args.Payload),
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
                        new StatelessServiceDelegateInvocationContextOnDataPackageRemoved(args.Payload),
                        args.CancellationToken);

                    args.Complete();
                }
                catch (Exception e)
                {
                    args.Fail(e);
                }
            };
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            this.serviceEvents.NotifyStartupAsync(default).GetAwaiter().GetResult();

            return this.GetServiceListeners();
        }

        protected override async Task RunAsync(
            CancellationToken cancellationToken)
        {
            await this.serviceEvents.NotifyRunAsync(cancellationToken);
        }

        protected override async Task OnCloseAsync(
            CancellationToken cancellationToken)
        {
            var payload = new StatelessServiceEventPayloadOnShutdown(false);

            await this.serviceEvents.NotifyShutdownAsync(payload, cancellationToken);
        }

        protected override void OnAbort()
        {
            var payload = new StatelessServiceEventPayloadOnShutdown(true);

            this.serviceEvents.NotifyShutdownAsync(payload, default).GetAwaiter().GetResult();
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
            return this.serviceListeners ?? Array.Empty<ServiceInstanceListener>();
        }

        private IEnumerable<StatelessServiceDelegate> GetServiceDelegates(
            StatelessServiceLifecycleEvent @event)
        {
            return this.serviceDelegates == null
                ? Array.Empty<StatelessServiceDelegate>()
                : this.serviceDelegates[@event];
        }

        private StatelessServiceEventSource GetServiceEventSource()
        {
            return this.serviceEventSource;
        }
    }
}