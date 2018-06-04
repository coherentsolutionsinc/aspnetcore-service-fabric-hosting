using System;
using System.Diagnostics.Tracing;
using System.Fabric;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common.Exceptions;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public abstract class ServiceHostAspNetCoreListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        : ServiceHostListenerReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostAspNetCoreListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
        protected abstract class Parameters
            : IServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        {
            public string EndpointName { get; private set; }

            public ServiceFabricIntegrationOptions IntegrationOptions { get; private set; }

            public Func<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>, AspNetCoreCommunicationListener>
                AspNetCoreCommunicationListenerFunc { get; private set; }

            public Func<IWebHostBuilderExtensionsImpl> WebHostBuilderExtensionsImplFunc { get; private set; }

            public Func<IWebHostExtensionsImpl> WebHostExtensionsImplFunc { get; private set; }

            public Func<IWebHostBuilder> WebHostBuilderFunc { get; private set; }

            public Action<IWebHostBuilder> WebHostConfigAction { get; private set; }

            protected Parameters()
            {
                this.EndpointName = string.Empty;
                this.IntegrationOptions = ServiceFabricIntegrationOptions.None;
                this.AspNetCoreCommunicationListenerFunc = null;
                this.WebHostBuilderExtensionsImplFunc = DefaultWebHostBuilderExtensionsImplFunc;
                this.WebHostExtensionsImplFunc = DefaultWebHostExtensionsImplFunc;
                this.WebHostBuilderFunc = DefaultWebHostBuilderFunc;
                this.WebHostConfigAction = DefaultWebHostConfigAction;
            }

            public void UseEndpointName(
                string endpointName)
            {
                this.EndpointName = endpointName
                 ?? throw new ArgumentNullException(nameof(endpointName));
            }

            public void UseIntegrationOptions(
                ServiceFabricIntegrationOptions integrationOptions)
            {
                this.IntegrationOptions = integrationOptions;
            }

            public void UseWebHostBuilderExtensionsImpl(
                Func<IWebHostBuilderExtensionsImpl> factoryFunc)
            {
                this.WebHostBuilderExtensionsImplFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseWebHostExtensionsImpl(
                Func<IWebHostExtensionsImpl> factoryFunc)
            {
                this.WebHostExtensionsImplFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseWebHostBuilder(
                Func<IWebHostBuilder> factoryFunc)
            {
                this.WebHostBuilderFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseAspNetCoreCommunicationListener(
                Func<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>, AspNetCoreCommunicationListener> factoryFunc)
            {
                this.AspNetCoreCommunicationListenerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void ConfigureWebHost(
                Action<IWebHostBuilder> configAction)
            {
                if (configAction == null)
                {
                    throw new ArgumentNullException(nameof(configAction));
                }

                this.WebHostConfigAction = this.WebHostConfigAction.Chain(configAction);
            }

            private static IWebHostBuilderExtensionsImpl DefaultWebHostBuilderExtensionsImplFunc()
            {
                return new WebHostBuilderExtensionsImpl();
            }

            private static IWebHostExtensionsImpl DefaultWebHostExtensionsImplFunc()
            {
                return new WebHostExtensionsImpl();
            }

            private static IWebHostBuilder DefaultWebHostBuilderFunc()
            {
                return new WebHostBuilder();
            }

            private static void DefaultWebHostConfigAction(
                IWebHostBuilder builder)
            {
            }
        }

        public abstract TListener Activate(
            TService service);

        protected Func<ServiceContext, AspNetCoreCommunicationListener> CreateAspNetCoreCommunicationListenerFunc(
            TService service,
            TParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.AspNetCoreCommunicationListenerFunc == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(parameters.AspNetCoreCommunicationListenerFunc)} was configured");
            }

            var build = new Func<string, AspNetCoreCommunicationListener, IWebHost>(
                (
                    url,
                    listener) =>
                {
                    var serviceContext = service.GetContext();
                    var servicePartition = service.GetPartition();
                    var serviceEventSource = service.GetEventSource();

                    var builder = parameters.WebHostBuilderFunc();
                    if (builder == null)
                    {
                        throw new FactoryProducesNullInstanceException<IWebHostBuilder>();
                    }

                    builder = new ExtensibleWebHostBuilder(builder);

                    parameters.WebHostConfigAction(builder);

                    var extensionsImpl = parameters.WebHostBuilderExtensionsImplFunc();
                    if (extensionsImpl == null)
                    {
                        throw new FactoryProducesNullInstanceException<IWebHostBuilderExtensionsImpl>();
                    }

                    extensionsImpl.UseServiceFabricIntegration(builder, listener, parameters.IntegrationOptions);
                    extensionsImpl.UseUrls(builder, url);

                    // This is important to let UseServiceFabricIntegration execute first - otherwise listener.UrlSuffix would be an empty string.
                    var listenerInformation = new ServiceAspNetCoreListenerInformation(
                        parameters.EndpointName,
                        listener.UrlSuffix);

                    builder.ConfigureServices(
                        services =>
                        {
                            // This is used for service type agnostic code
                            services.Add(new ServiceDescriptor(typeof(ServiceContext), serviceContext));
                            services.Add(new ServiceDescriptor(typeof(IServicePartition), servicePartition));
                            services.Add(new ServiceDescriptor(typeof(IServiceEventSource), serviceEventSource));
                            services.Add(new ServiceDescriptor(typeof(IServiceListenerInformation), listenerInformation));

                            // This is used for service type dependent code
                            switch (serviceContext)
                            {
                                case StatefulServiceContext _:
                                    services.Add(new ServiceDescriptor(typeof(StatefulServiceContext), serviceContext));
                                    services.Add(new ServiceDescriptor(typeof(IStatefulServicePartition), servicePartition));
                                    break;
                                case StatelessServiceContext _:
                                    services.Add(new ServiceDescriptor(typeof(StatelessServiceContext), serviceContext));
                                    services.Add(new ServiceDescriptor(typeof(IStatelessServicePartition), servicePartition));
                                    break;
                            }
                            services.Add(new ServiceDescriptor(typeof(IServiceAspNetCoreListenerInformation), listenerInformation));
                        });

                    // Configure logging provider
                    builder.ConfigureLogging(
                        config =>
                        {
                            config.AddProvider(new ServiceAspNetCoreListenerLoggerProvider(listenerInformation, serviceEventSource));
                        });

                    return new ExtensibleWebHost(builder.Build());
                });

            return context => parameters.AspNetCoreCommunicationListenerFunc(context, parameters.EndpointName, build);
        }
    }
}