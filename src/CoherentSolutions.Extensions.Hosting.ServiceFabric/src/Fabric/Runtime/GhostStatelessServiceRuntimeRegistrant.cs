using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories;
using Microsoft.Extensions.Logging.Console;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class GhostStatelessServiceRuntimeRegistrant : IStatelessServiceRuntimeRegistrant
    {
        public async Task RegisterAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            CancellationToken cancellationToken)
        {
            var (context, partition) = GhostServiceRuntime.CreateStatelessServiceContextAndPartition(serviceTypeName);

            await new GhostStatelessServiceInstance(
                serviceFactory(context), 
                partition,
                new ConsoleLogger(serviceTypeName, null, true)).StartupAsync();
        }
        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}