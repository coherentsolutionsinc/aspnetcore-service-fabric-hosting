using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHost : IStatelessServiceHost
    {
        private readonly string serviceName;

        private readonly IServiceProvider serviceDependencies;

        private readonly IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators;

        public StatelessServiceHost(
            string serviceName,
            IServiceProvider serviceDependencies,
            IReadOnlyList<IStatelessServiceHostListenerReplicator> serviceListenerReplicators)
        {
            this.serviceName = serviceName
             ?? throw new ArgumentNullException(nameof(serviceName));

            this.serviceDependencies = serviceDependencies 
             ?? throw new ArgumentNullException(nameof(serviceDependencies));

            this.serviceListenerReplicators = serviceListenerReplicators
             ?? Array.Empty<IStatelessServiceHostListenerReplicator>();
        }

        public async Task StartAsync(
            CancellationToken cancellationToken)
        {
            await ServiceRuntime.RegisterServiceAsync(
                this.serviceName,
                serviceContext => new StatelessService(serviceContext, this.serviceDependencies, this.serviceListenerReplicators),
                cancellationToken: cancellationToken);
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}