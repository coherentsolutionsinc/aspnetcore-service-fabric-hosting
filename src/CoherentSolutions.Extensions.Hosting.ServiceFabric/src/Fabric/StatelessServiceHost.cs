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

        private readonly IReadOnlyList<IStatelessServiceHostDelegateReplicator> serviceDelegateReplicators;

        private readonly IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators;

        public StatelessServiceHost(
            string serviceTypeName,
            IStatelessServiceRuntimeRegistrant serviceRuntimeRegistrant,
            IReadOnlyList<IStatelessServiceHostDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators)
        {
            this.serviceTypeName = serviceTypeName
             ?? throw new ArgumentNullException(nameof(serviceTypeName));

            this.serviceRuntimeRegistrant = serviceRuntimeRegistrant
             ?? throw new ArgumentNullException(nameof(serviceRuntimeRegistrant));

            this.serviceDelegateReplicators = serviceDelegateReplicators
             ?? throw new ArgumentNullException(nameof(serviceDelegateReplicators));

            this.serviceListenerReplicators = serviceListenerReplicators
             ?? Array.Empty<IStatelessServiceHostListenerReplicator>();
        }

        public async Task StartAsync(
            CancellationToken cancellationToken)
        {
            await this.serviceRuntimeRegistrant.RegisterAsync(
                this.serviceTypeName,
                serviceContext => new StatelessService(
                    serviceContext,
                    this.serviceDelegateReplicators,
                    this.serviceListenerReplicators),
                cancellationToken);
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}