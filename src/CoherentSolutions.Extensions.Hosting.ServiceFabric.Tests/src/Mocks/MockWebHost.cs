using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockWebHost : IWebHost
    {
        public IFeatureCollection ServerFeatures { get; }

        public IServiceProvider Services { get; }

        public MockWebHost(
            IFeatureCollection serverFeatures,
            IServiceProvider services)
        {
            this.ServerFeatures = serverFeatures;
            this.Services = services;
        }

        public void Dispose()
        {
        }

        public void Start()
        {
        }

        public Task StartAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }
    }
}