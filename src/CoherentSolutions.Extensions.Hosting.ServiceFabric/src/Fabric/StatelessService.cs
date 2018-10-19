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
            this.serviceEvents = new ServiceEvents();

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
                    var context = new StatelessServiceDelegateInvocationContextOnShutdown(args.Payload);

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
            return this.serviceListeners ?? Enumerable.Empty<ServiceInstanceListener>();
        }

        private IEnumerable<StatelessServiceDelegate> GetServiceDelegates(
            StatelessServiceLifecycleEvent @event)
        {
            return this.serviceDelegates == null
                ? Enumerable.Empty<StatelessServiceDelegate>()
                : this.serviceDelegates[@event];
        }

        private StatelessServiceEventSource GetServiceEventSource()
        {
            return this.serviceEventSource;
        }
    }
}