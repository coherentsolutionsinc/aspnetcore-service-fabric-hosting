using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHost : IStatefulServiceHost
    {
        private readonly string serviceName;


        private readonly IEnumerable<IStatefulServiceHostListenerReplicator> listenerReplicators;

        public StatefulServiceHost(
            string serviceName,
            IEnumerable<IStatefulServiceHostListenerReplicator> listenerReplicators)
        {
            this.serviceName = serviceName
             ?? throw new ArgumentNullException(nameof(serviceName));

            this.listenerReplicators = listenerReplicators
             ?? Enumerable.Empty<IStatefulServiceHostListenerReplicator>();
        }

        public async Task StartAsync(
            CancellationToken cancellationToken)
        {
            await ServiceRuntime.RegisterServiceAsync(
                this.serviceName,
                serviceContext => new StatefulService(serviceContext, this.listenerReplicators),
                cancellationToken: cancellationToken);
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}