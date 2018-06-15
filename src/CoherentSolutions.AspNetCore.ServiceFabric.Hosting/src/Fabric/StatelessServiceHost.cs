using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatelessServiceHost : IStatelessServiceHost
    {
        private readonly string serviceName;

        private readonly IEnumerable<IStatelessServiceHostListenerReplicator> listenerReplicators;

        public StatelessServiceHost(
            string serviceName,
            IEnumerable<IStatelessServiceHostListenerReplicator> listenerReplicators)
        {
            this.serviceName = serviceName
             ?? throw new ArgumentNullException(nameof(serviceName));

            this.listenerReplicators = listenerReplicators
             ?? Enumerable.Empty<IStatelessServiceHostListenerReplicator>();
        }

        public async Task StartAsync(
            CancellationToken cancellationToken)
        {
            await ServiceRuntime.RegisterServiceAsync(
                this.serviceName,
                serviceContext => new StatelessService(serviceContext, this.listenerReplicators),
                cancellationToken: cancellationToken);
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}