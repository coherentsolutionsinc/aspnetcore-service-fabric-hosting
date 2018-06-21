using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHost : IStatelessServiceHost
    {
        private readonly string serviceName;

        private readonly IReadOnlyList<IStatelessServiceHostDelegateReplicator> serviceDelegateReplicators;

        private readonly IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators;

        public StatelessServiceHost(
            string serviceName,
            IReadOnlyList<IStatelessServiceHostDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators)
        {
            this.serviceName = serviceName
             ?? throw new ArgumentNullException(nameof(serviceName));

            this.serviceDelegateReplicators = serviceDelegateReplicators
             ?? throw new ArgumentNullException(nameof(serviceDelegateReplicators));

            this.serviceListenerReplicators = serviceListenerReplicators
             ?? Array.Empty<IStatelessServiceHostListenerReplicator>();
        }

        public async Task StartAsync(
            CancellationToken cancellationToken)
        {
            await ServiceRuntime.RegisterServiceAsync(
                this.serviceName,
                serviceContext => new StatelessService(
                    serviceContext,
                    this.serviceDelegateReplicators,
                    this.serviceListenerReplicators),
                cancellationToken: cancellationToken);
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}