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


        private readonly IReadOnlyList<IStatefulServiceHostAsyncDelegateReplicator> serviceDelegatesReplicators;

        private readonly IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators;

        public StatefulServiceHost(
            string serviceName,
            IReadOnlyList<IStatefulServiceHostAsyncDelegateReplicator> serviceDelegatesReplicators,
            IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators)
        {
            this.serviceName = serviceName
             ?? throw new ArgumentNullException(nameof(serviceName));

            this.serviceDelegatesReplicators = serviceDelegatesReplicators 
             ?? throw new ArgumentNullException(nameof(serviceDelegatesReplicators));

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
                    this.serviceDelegatesReplicators,
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