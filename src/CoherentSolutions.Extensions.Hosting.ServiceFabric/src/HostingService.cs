using System;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.Extensions.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric
{
    public class HostingService : IHostedService
    {
        private readonly IServiceHost host;

        public HostingService(
            IServiceHost host)
        {
            this.host = host
             ?? throw new ArgumentNullException(nameof(host));
        }

        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            return this.host.StartAsync(cancellationToken);
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            return this.host.StopAsync(cancellationToken);
        }
    }
}