using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceRuntimeRegistrant : IStatelessServiceRuntimeRegistrant
    {
        private readonly IServiceHostRuntime serviceHostRuntime;

        public StatelessServiceRuntimeRegistrant(
            IServiceHostRuntime serviceHostRuntime)
        {
            this.serviceHostRuntime = serviceHostRuntime ?? throw new ArgumentNullException(nameof(serviceHostRuntime));
        }

        public Task RegisterAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
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