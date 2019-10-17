using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

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
            var runtime = GhostServiceRuntime.Default;
            var context = new StatelessServiceContext(
                runtime.GetNodeContext(),
                runtime.GetCodePackageActivationContext(),
                serviceTypeName,
                runtime.CreateServiceName(),
                null,
                Guid.NewGuid(),
                runtime.CreateInstanceId());

            var service = serviceFactory(context);
            var partition = new GhostStatelessServiceSingletonPartition();
            var logger = new ConsoleLogger(serviceTypeName, null, true);

            var instance = new GhostStatelessServiceInstance(service, partition, logger);

            await instance.StartupAsync();
        }

        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}