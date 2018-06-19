using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        private readonly IServiceProvider serviceDependencies;

        private readonly IEnumerable<IStatefulServiceHostListenerReplicator> serviceListenerReplicators;

        private readonly ServiceEventSource eventSource;

        private readonly EventSynchronization eventSynchronization;

        public StatefulService(
            StatefulServiceContext serviceContext,
            IServiceProvider serviceDependencies,
            IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators)
            : base(serviceContext)
        {
            this.eventSource = new ServiceEventSource(
                serviceContext,
                $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}",
                EventSourceSettings.EtwSelfDescribingEventFormat);

            this.eventSynchronization = new EventSynchronization();

            this.serviceDependencies = serviceDependencies 
             ?? throw new ArgumentNullException(nameof(serviceDependencies));

            this.serviceListenerReplicators = serviceListenerReplicators 
             ?? throw new ArgumentNullException(nameof(serviceListenerReplicators));
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.serviceListenerReplicators.Select(replicator =>replicator.ReplicateFor(this));
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
            // Wait when all listeners are opened
            await this.eventSynchronization.WhenAllListenersOpened(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Run async operations
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
    }
}