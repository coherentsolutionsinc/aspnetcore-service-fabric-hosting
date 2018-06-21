using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHost : IStatefulServiceHost
    {
        private readonly string serviceName;

        private readonly IReadOnlyList<IStatefulServiceHostDelegateReplicator> serviceDelegateReplicators;

        private readonly IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators;

        public StatefulServiceHost(
            string serviceName,
            IReadOnlyList<IStatefulServiceHostDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators)
        {
            this.serviceName = serviceName
             ?? throw new ArgumentNullException(nameof(serviceName));

            this.serviceDelegateReplicators = serviceDelegateReplicators
             ?? throw new ArgumentNullException(nameof(serviceDelegateReplicators));

            this.serviceListenerReplicators = serviceListenerReplicators
             ?? Array.Empty<IStatefulServiceHostListenerReplicator>();
        }

        public async Task StartAsync(
            CancellationToken cancellationToken)
        {
            await ServiceRuntime.RegisterServiceAsync(
                this.serviceName,
                serviceContext => new StatefulService(
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