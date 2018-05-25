using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common
{
    public class ExtensibleWebHostBuilder : IWebHostBuilder
    {
        private readonly IWebHostBuilder successor;

        public ExtensibleWebHostBuilder(
            IWebHostBuilder successor)
        {
            this.successor = successor
             ?? throw new ArgumentNullException(nameof(successor));
        }

        public string GetSetting(
            string key)
        {
            return this.successor.GetSetting(key);
        }

        public IWebHostBuilder ConfigureAppConfiguration(
            Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            return this.successor.ConfigureAppConfiguration(configureDelegate);
        }

        public IWebHostBuilder ConfigureServices(
            Action<IServiceCollection> configureServices)
        {
            return this.successor.ConfigureServices(configureServices);
        }

        public IWebHostBuilder ConfigureServices(
            Action<WebHostBuilderContext, IServiceCollection> configureServices)
        {
            return this.successor.ConfigureServices(configureServices);
        }

        public IWebHostBuilder UseSetting(
            string key,
            string value)
        {
            return this.successor.UseSetting(key, value);
        }

        public IWebHost Build()
        {
            var webHost = this.successor.Build();

            return webHost is ExtensibleWebHost
                ? webHost
                : new ExtensibleWebHost(webHost);
        }
    }
}