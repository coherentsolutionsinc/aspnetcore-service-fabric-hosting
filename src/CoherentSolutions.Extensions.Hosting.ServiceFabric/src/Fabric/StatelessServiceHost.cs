using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHost : IStatelessServiceHost
    {
        private readonly string serviceTypeName;

        private readonly IStatelessServiceRuntimeRegistrant serviceRuntimeRegistrant;

        private readonly IStatelessServiceHostEventSourceReplicator serviceEventSourceReplicator;

        private readonly IReadOnlyList<IStatelessServiceHostDelegateReplicator> serviceDelegateReplicators;

        private readonly IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators;

        public StatelessServiceHost(
            string serviceTypeName,
            IStatelessServiceRuntimeRegistrant serviceRuntimeRegistrant,
            IStatelessServiceHostEventSourceReplicator serviceEventSourceReplicator,
            IReadOnlyList<IStatelessServiceHostDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators)
        {
            this.serviceTypeName = serviceTypeName
             ?? throw new ArgumentNullException(nameof(serviceTypeName));

            this.serviceRuntimeRegistrant = serviceRuntimeRegistrant
             ?? throw new ArgumentNullException(nameof(serviceRuntimeRegistrant));

            this.serviceEventSourceReplicator = serviceEventSourceReplicator
             ?? throw new ArgumentNullException(nameof(serviceEventSourceReplicator));

            this.serviceDelegateReplicators = serviceDelegateReplicators
             ?? throw new ArgumentNullException(nameof(serviceDelegateReplicators));

            this.serviceListenerReplicators = serviceListenerReplicators
             ?? Array.Empty<IStatelessServiceHostListenerReplicator>();
        }

        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            return this.serviceRuntimeRegistrant.RegisterAsync(
                this.serviceTypeName,
                serviceContext => new StatelessService(
                    serviceContext,
                    this.serviceEventSourceReplicator,
                    this.serviceDelegateReplicators,
                    this.serviceListenerReplicators),
                cancellationToken);
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            return this.serviceRuntimeRegistrant.UnregisterAsync(
                this.serviceTypeName,
                cancellationToken);
        }
    }
}