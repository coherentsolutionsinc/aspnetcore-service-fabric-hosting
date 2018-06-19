﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessService : Microsoft.ServiceFabric.Services.Runtime.StatelessService, IStatelessService
    {
        private readonly IServiceProvider serviceDependencies;

        private readonly IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators;

        private readonly ServiceEventSource eventSource;

        private readonly StatelessServiceEventSynchronization eventSynchronization;

        public StatelessService(
            StatelessServiceContext serviceContext,
            IServiceProvider serviceDependencies,
            IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators)
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

            this.eventSynchronization = new StatelessServiceEventSynchronization(
                serviceListenerReplicators.Count);

            this.serviceDependencies = serviceDependencies;
            this.serviceListenerReplicators = serviceListenerReplicators;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.serviceListenerReplicators
               .Select(replicator =>
                {
                    var replicaListener = replicator.ReplicateFor(this);
                    return new ServiceInstanceListener(
                        context =>
                        {
                            return new ServiceCommunicationListenerEventDecorator(
                                this.eventSynchronization,
                                replicaListener.CreateCommunicationListener(context));
                        },
                        replicaListener.Name);
                });
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