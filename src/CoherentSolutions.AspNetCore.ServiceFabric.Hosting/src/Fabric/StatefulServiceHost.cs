using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
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

        public void Run()
        {
            ServiceRuntime.RegisterServiceAsync(
                    this.serviceName,
                    serviceContext => new StatefulService(serviceContext, this.listenerReplicators))
               .GetAwaiter()
               .GetResult();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}