using System;
using System.Collections.Generic;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions.Support;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base
{
    public abstract class ServiceAspNetCoreListenerTheoryItem<T>
        : TheoryItem,
          IUseWebHostBuilderTheoryExtensionSupported,
          IResolveDependencyTheoryExtensionSupported
        where T : IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
        private class WebHostBuilderProxy : IWebHostBuilder
        {
            private readonly IWebHostBuilder webHostBuilderImplementation;

            private readonly IEnumerable<Action<IServiceProvider>> actions;

            public WebHostBuilderProxy(
                IWebHostBuilder webHostBuilderImplementation,
                IEnumerable<Action<IServiceProvider>> actions)
            {
                this.webHostBuilderImplementation = webHostBuilderImplementation;
                this.actions = actions;
            }

            public IWebHost Build()
            {
                var host = this.webHostBuilderImplementation.Build();
                foreach (var action in this.actions)
                {
                    action(host.Services);
                }

                return host;
            }

            public IWebHostBuilder ConfigureAppConfiguration(
                Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
            {
                return this.webHostBuilderImplementation.ConfigureAppConfiguration(configureDelegate);
            }

            public IWebHostBuilder ConfigureServices(
                Action<IServiceCollection> configureServices)
            {
                return this.webHostBuilderImplementation.ConfigureServices(configureServices);
            }

            public IWebHostBuilder ConfigureServices(
                Action<WebHostBuilderContext, IServiceCollection> configureServices)
            {
                return this.webHostBuilderImplementation.ConfigureServices(configureServices);
            }

            public string GetSetting(
                string key)
            {
                return this.webHostBuilderImplementation.GetSetting(key);
            }

            public IWebHostBuilder UseSetting(
                string key,
                string value)
            {
                return this.webHostBuilderImplementation.UseSetting(key, value);
            }
        }

        protected ServiceAspNetCoreListenerTheoryItem(
            string name)
            : base(name)
        {
            this.StartExtensionsSetup();

            this.SetupExtension(new UseWebHostBuilderTheoryExtension());
            this.SetupExtension(new ResolveDependencyTheoryExtension());

            this.StopExtensionsSetup();
        }

        protected virtual void ConfigureExtensions(
            T configurator)
        {
            var useWebHostBuilderExtension = this.GetExtension<IUseWebHostBuilderTheoryExtension>();
            var resolveDependenciesExtension = this.GetExtension<IResolveDependencyTheoryExtension>();

            configurator.UseWebHostBuilder(
                () =>
                {
                    return new WebHostBuilderProxy(
                        useWebHostBuilderExtension.Factory(),
                        resolveDependenciesExtension.ServiceResolveDelegates);
                });
        }
    }
}