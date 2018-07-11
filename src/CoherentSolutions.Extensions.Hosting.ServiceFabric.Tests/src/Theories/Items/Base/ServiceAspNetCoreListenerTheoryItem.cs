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
        : ServiceListenerTheoryItem<T>,
          IUseListenerEndpointTheoryExtensionSupported,
          IUseWebHostBuilderTheoryExtensionSupported,
          IPickDependencyTheoryExtensionSupported
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
        }

        protected override void InitializeExtensions()
        {
            base.InitializeExtensions();

            this.SetupExtension(new UseAspNetCoreCommunicationListenerTheoryExtension());
            this.SetupExtension(new UseWebHostBuilderTheoryExtension());
            this.SetupExtension(new PickDependencyTheoryExtension());
            this.SetupExtension(new PickListenerEndpointTheoryExtension());
        }

        protected override void ConfigureExtensions(
            T configurator)
        {
            base.ConfigureExtensions(configurator);

            var useAspNetCoreCommunicationListenerExtension = this.GetExtension<IUseAspNetCoreCommunicationListenerTheoryExtension>();
            var useWebHostBuilderExtension = this.GetExtension<IUseWebHostBuilderTheoryExtension>();
            var pickDependenciesExtension = this.GetExtension<IPickDependencyTheoryExtension>();
            var pickListenerEndpointExtensions = this.GetExtension<IPickListenerEndpointTheoryExtension>();

            configurator.UseCommunicationListener(
                (
                    context,
                    endpointName,
                    factory) =>
                {
                    pickListenerEndpointExtensions.PickAction(endpointName);

                    return useAspNetCoreCommunicationListenerExtension.Factory(context, endpointName, factory);
                });
            configurator.UseWebHostBuilder(
                () =>
                {
                    return new WebHostBuilderProxy(
                        useWebHostBuilderExtension.Factory(),
                        pickDependenciesExtension.PickActions);
                });
        }
    }
}