using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceRuntimeRegistrant : IStatefulServiceRuntimeRegistrant
    {
        private readonly IServiceHostRuntime serviceHostRuntime;

        public StatefulServiceRuntimeRegistrant(
            IServiceHostRuntime serviceHostRuntime)
        {
            this.serviceHostRuntime = serviceHostRuntime ?? throw new ArgumentNullException(nameof(serviceHostRuntime));
        }

        public Task RegisterAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulService> serviceFactory,
            CancellationToken cancellationToken)
        {
            return this.serviceHostRuntime.RegisterServiceAsync(
                serviceTypeName, 
                serviceFactory, 
                cancellationToken: cancellationToken);
        }

        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            // currently we do nothing here.

            return Task.CompletedTask;
        }
    }
}