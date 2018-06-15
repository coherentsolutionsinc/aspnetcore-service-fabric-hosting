using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Stubs
{
    internal class WebHostBuilderStub : IWebHostBuilder
    {
        private class WebHostStub : IWebHost
        {
            public IFeatureCollection ServerFeatures => new FeatureCollection();

            public IServiceProvider Services => new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection());

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

        public IWebHost Build()
        {
            return new WebHostStub();
        }

        public IWebHostBuilder ConfigureAppConfiguration(
            Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            return this;
        }

        public IWebHostBuilder ConfigureServices(
            Action<IServiceCollection> configureServices)
        {
            return this;
        }

        public IWebHostBuilder ConfigureServices(
            Action<WebHostBuilderContext, IServiceCollection> configureServices)
        {
            return this;
        }

        public string GetSetting(
            string key)
        {
            return string.Empty;
        }

        public IWebHostBuilder UseSetting(
            string key,
            string value)
        {
            return this;
        }

        public static IWebHostBuilder Func()
        {
            return new WebHostBuilderStub();
        }
    }
}