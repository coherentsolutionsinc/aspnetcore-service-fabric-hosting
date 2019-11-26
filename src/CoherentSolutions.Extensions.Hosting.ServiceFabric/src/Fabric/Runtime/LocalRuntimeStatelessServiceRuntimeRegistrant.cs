using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class LocalRuntimeStatelessServiceRuntimeRegistrant : IStatelessServiceRuntimeRegistrant
    {
        public async Task RegisterAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            CancellationToken cancellationToken)
        {
            await LocalRuntime.RegisterServiceAsync(
                serviceTypeName,
                serviceFactory,
                cancellationToken: cancellationToken);
        }
        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            // Implement shutdown routine for services.
            return Task.CompletedTask;
        }
    }
}