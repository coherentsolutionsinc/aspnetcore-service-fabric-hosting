using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHost : IStatefulServiceHost
    {
        private readonly string serviceTypeName;

        private readonly IStatefulServiceRuntimeRegistrant serviceRuntimeRegistrant;

        private readonly IReadOnlyList<IStatefulServiceHostDelegateReplicator> serviceDelegateReplicators;

        private readonly IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators;

        public StatefulServiceHost(
            string serviceTypeName,
            IStatefulServiceRuntimeRegistrant serviceRuntimeRegistrant,
            IReadOnlyList<IStatefulServiceHostDelegateReplicator> serviceDelegateReplicators,
            IReadOnlyList<IStatefulServiceHostListenerReplicator> serviceListenerReplicators)
        {
            this.serviceTypeName = serviceTypeName
             ?? throw new ArgumentNullException(nameof(serviceTypeName));

            this.serviceRuntimeRegistrant = serviceRuntimeRegistrant
             ?? throw new ArgumentNullException(nameof(serviceRuntimeRegistrant));

            this.serviceDelegateReplicators = serviceDelegateReplicators
             ?? throw new ArgumentNullException(nameof(serviceDelegateReplicators));

            this.serviceListenerReplicators = serviceListenerReplicators
             ?? Array.Empty<IStatefulServiceHostListenerReplicator>();
        }

        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            return this.serviceRuntimeRegistrant.RegisterAsync(
                this.serviceTypeName,
                serviceContext => new StatefulService(
                    serviceContext,
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