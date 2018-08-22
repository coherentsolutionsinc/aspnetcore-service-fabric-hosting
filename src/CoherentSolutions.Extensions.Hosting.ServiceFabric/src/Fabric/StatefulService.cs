using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulService : Microsoft.ServiceFabric.Services.Runtime.StatefulService, IStatefulService
    {
        private class EventSynchronization
        {
            private TaskCompletionSource<int> whenRoleDetermined;

            public EventSynchronization()
            {
                this.whenRoleDetermined = new TaskCompletionSource<int>();
            }

            public void NotifyRoleDetermined(
                ReplicaRole toRole)
            {
                // Initialization or promotion - we can reuse existing Task.
                if (toRole == ReplicaRole.Primary)
                {
                    this.whenRoleDetermined.SetResult(0);
                }
                else
                {
                    this.whenRoleDetermined = new TaskCompletionSource<int>();
                }
            }

            public async Task WhenAllListenersOpened(
                CancellationToken cancellationToken)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var allSource = this.whenRoleDetermined;
                using (cancellationToken.Register(
                    () =>
                    {
                        allSource.SetCanceled();
                    }))
                {
                    await allSource.Task;
                }
            }
        }

        private readonly ServiceEventSource eventSource;

        private readonly EventSynchronization eventSynchronization;

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

            this.eventSynchronization = new EventSynchronization();

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
                   .Select(replicator => replicator.ReplicateFor(this))
                   .ToList();
            }
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.GetServiceListeners();
        }

        protected override Task OnChangeRoleAsync(
            ReplicaRole newRole,
            CancellationToken cancellationToken)
        {
            this.eventSynchronization.NotifyRoleDetermined(newRole);

            return Task.CompletedTask;
        }

        protected override async Task RunAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.eventSynchronization.WhenAllListenersOpened(cancellationToken);

            await this.InvokeDelegates(StatefulServiceLifecycleEvent.OnRunAfterListenersAreOpened, cancellationToken);
        }

        protected override async Task OnOpenAsync(
            ReplicaOpenMode openMode,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.InvokeDelegates(StatefulServiceLifecycleEvent.OnOpen, cancellationToken);
        }

        protected override async Task OnCloseAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.InvokeDelegates(StatefulServiceLifecycleEvent.OnClose, cancellationToken);
        }

        protected override void OnAbort()
        {
            Task.Run(async () => await this.InvokeDelegates(StatefulServiceLifecycleEvent.OnAbort)).Wait();
        }

        protected override async Task<bool> OnDataLossAsync(
            RestoreContext restoreCtx,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.InvokeDelegates(StatefulServiceLifecycleEvent.OnDataLoss, cancellationToken);

            return await base.OnDataLossAsync(restoreCtx, cancellationToken);
        }

        protected override async Task OnRestoreCompletedAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.InvokeDelegates(StatefulServiceLifecycleEvent.OnRestoreCompleted, cancellationToken);

            await base.OnRestoreCompletedAsync(cancellationToken);
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
            StatefulServiceLifecycleEvent @event,
            CancellationToken cancellationToken = default)
        {
            var context = new StatefulServiceDelegateInvocationContext(@event);
            var delegates = this.GetServiceDelegates(@event);
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