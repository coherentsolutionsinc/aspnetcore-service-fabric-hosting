using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceRuntimeRegistrant : IStatelessServiceRuntimeRegistrant
    {
        public Task RegisterAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            CancellationToken cancellationToken)
        {
            return ServiceRuntime.RegisterServiceAsync(
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