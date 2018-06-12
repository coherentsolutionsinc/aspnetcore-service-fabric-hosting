using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common
{
    public class ExtensibleWebHost : IWebHost
    {
        private readonly IWebHost successor;

        public IFeatureCollection ServerFeatures => this.successor.ServerFeatures;

        public IServiceProvider Services => this.successor.Services;

        public ExtensibleWebHost(
            IWebHost successor)
        {
            this.successor = successor
             ?? throw new ArgumentNullException(nameof(successor));
        }

        public void Start()
        {
            this.ExecuteOnRunActions();

            this.successor.Start();
        }

        public Task StartAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            this.ExecuteOnRunActions();

            return this.successor.StartAsync(cancellationToken);
        }

        public Task StopAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            return this.successor.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            this.successor.Dispose();
        }

        private void ExecuteOnRunActions()
        {
            var actions = this.Services.GetServices<IExtensibleWebHostOnRunAction>();
            foreach (var action in actions)
            {
                action.Execute(this);
            }
        }
    }
}