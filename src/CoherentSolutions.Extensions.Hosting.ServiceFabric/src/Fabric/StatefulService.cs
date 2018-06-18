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
        private readonly IServiceProvider serviceDependencies;

        private readonly IEnumerable<IStatefulServiceHostListenerReplicator> serviceListenerReplicators;

        private readonly ServiceEventSource eventSource;

        private readonly StatefulServiceEventSynchronization eventSynchronization;

        public StatefulService(
            StatefulServiceContext serviceContext,
            IServiceProvider serviceDependencies,
            IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators)
            : base(serviceContext)
        {
            if (serviceDependencies == null)
            {
                throw new ArgumentNullException(nameof(serviceDependencies));
            }

            if (serviceListenerReplicators == null)
            {
                throw new ArgumentNullException(nameof(serviceListenerReplicators));
            }

            this.eventSource = new ServiceEventSource(
                serviceContext,
                $"{serviceContext.CodePackageActivationContext.ApplicationTypeName}.{serviceContext.ServiceTypeName}",
                EventSourceSettings.EtwSelfDescribingEventFormat);

            this.eventSynchronization = new StatefulServiceEventSynchronization();

            this.serviceDependencies = serviceDependencies;
            this.serviceListenerReplicators = serviceListenerReplicators;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.serviceListenerReplicators
               .Select(replicator =>
                {
                    var replicaListener = replicator.ReplicateFor(this);
                    return new ServiceReplicaListener(
                        context =>
                        {
                            return new ServiceCommunicationListenerEventDecorator(
                                this.eventSynchronization,
                                replicaListener.CreateCommunicationListener(context));
                        },
                        replicaListener.Name,
                        replicaListener.ListenOnSecondary);
                });
        }

        protected override Task OnOpenAsync(
            ReplicaOpenMode openMode,
            CancellationToken cancellationToken)
        {
            return base.OnOpenAsync(openMode, cancellationToken);
        }

        protected override Task OnChangeRoleAsync(
            ReplicaRole newRole,
            CancellationToken cancellationToken)
        {
            return base.OnChangeRoleAsync(newRole, cancellationToken);
        }

        protected override Task RunAsync(
            CancellationToken cancellationToken)
        {
            return base.RunAsync(cancellationToken);
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