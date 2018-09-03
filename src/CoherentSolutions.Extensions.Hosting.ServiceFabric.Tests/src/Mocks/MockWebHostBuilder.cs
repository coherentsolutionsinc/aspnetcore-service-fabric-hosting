using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockWebHostBuilder : IWebHostBuilder
    {
        private sealed class Startup : IStartup
        {
            public IServiceProvider ConfigureServices(
                IServiceCollection services)
            {
                return new DefaultServiceProviderFactory().CreateServiceProvider(services);
            }

            public void Configure(
                IApplicationBuilder app)
            {
            }
        }

        private readonly Dictionary<string, string> settings;

        private readonly ServiceCollection services;

        public MockWebHostBuilder()
        {
            this.settings = new Dictionary<string, string>();
            this.services = new ServiceCollection();
            this.UseStartup<Startup>();
        }

        public IWebHost Build()
        {
            var provider = (IServiceProvider) this.services.BuildServiceProvider();
            var startup = provider.GetService<IStartup>();

            if (startup != null)
            {
                provider = startup.ConfigureServices(this.services);
            }

            return new MockWebHost(null, provider);
        }

        public IWebHostBuilder ConfigureAppConfiguration(
            Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            return this;
        }

        public IWebHostBuilder ConfigureServices(
            Action<IServiceCollection> configureServices)
        {
            configureServices(this.services);

            return this;
        }

        public IWebHostBuilder ConfigureServices(
            Action<WebHostBuilderContext, IServiceCollection> configureServices)
        {
            configureServices(null, this.services);

            return this;
        }

        public string GetSetting(
            string key)
        {
            return this.settings.TryGetValue(key, out var value)
                ? value
                : null;
        }

        public IWebHostBuilder UseSetting(
            string key,
            string value)
        {
            this.settings[key] = value;

            return this;
        }
    }
}