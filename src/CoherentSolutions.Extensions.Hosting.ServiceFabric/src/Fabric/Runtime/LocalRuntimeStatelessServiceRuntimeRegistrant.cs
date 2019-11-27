using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class LocalRuntimeStatelessServiceRuntimeRegistrant : IStatelessServiceRuntimeRegistrant
    {
        private readonly ILogger logger;

        public LocalRuntimeStatelessServiceRuntimeRegistrant(
            ILoggerProvider loggerProvider)
        {
            if (loggerProvider is null)
            {
                throw new ArgumentNullException(nameof(loggerProvider));
            }

            this.logger = loggerProvider.CreateLogger("Local Runtime");
        }

        public async Task RegisterAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            CancellationToken cancellationToken)
        {
            await LocalRuntime.RegisterServiceAsync(
                serviceTypeName,
                serviceFactory,
                this.logger,
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