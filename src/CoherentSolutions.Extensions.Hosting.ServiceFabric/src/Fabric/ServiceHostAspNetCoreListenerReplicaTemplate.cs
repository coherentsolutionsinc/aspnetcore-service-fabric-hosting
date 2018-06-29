using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostAspNetCoreListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        : ServiceHostListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        where TService : IService
        where TParameters : IServiceHostAspNetCoreListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
        protected abstract class AspNetCoreListenerParameters
            : ListenerParameters,
              IServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        {
            public ServiceFabricIntegrationOptions IntegrationOptions { get; private set; }

            public ServiceHostAspNetCoreCommunicationListenerFactory AspNetCoreCommunicationListenerFunc { get; private set; }

            public Func<IWebHostBuilder> WebHostBuilderFunc { get; private set; }

            public Action<IWebHostBuilder> WebHostConfigAction { get; private set; }

            protected AspNetCoreListenerParameters()
            {
                this.IntegrationOptions = ServiceFabricIntegrationOptions.None;
                this.AspNetCoreCommunicationListenerFunc = DefaultAspNetCoreCommunicationListenerFunc;
                this.WebHostBuilderFunc = DefaultWebHostBuilderFunc;
                this.WebHostConfigAction = DefaultWebHostConfigAction;
            }

            public void UseIntegrationOptions(
                ServiceFabricIntegrationOptions integrationOptions)
            {
                this.IntegrationOptions = integrationOptions;
            }

            public void UseWebHostBuilder(
                Func<IWebHostBuilder> factoryFunc)
            {
                this.WebHostBuilderFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseCommunicationListener(
                ServiceHostAspNetCoreCommunicationListenerFactory factoryFunc)
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

            private static AspNetCoreCommunicationListener DefaultAspNetCoreCommunicationListenerFunc(
                ServiceContext serviceContext,
                string endpointName,
                Func<string, AspNetCoreCommunicationListener, IWebHost> build)
            {
                var @delegate = new Func<string, AspNetCoreCommunicationListener, IWebHost>(build);
                return new KestrelCommunicationListener(serviceContext, endpointName, @delegate);
            }

            private static IWebHostBuilder DefaultWebHostBuilderFunc()
            {
                return WebHost.CreateDefaultBuilder();
            }

            private static void DefaultWebHostConfigAction(
                IWebHostBuilder builder)
            {
            }
        }

        protected override Func<ServiceContext, ICommunicationListener> CreateCommunicationListenerFunc(
            TService service,
            TParameters parameters)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
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

                    parameters.WebHostConfigAction(builder);

                    builder.UseServiceFabricIntegration(listener, parameters.IntegrationOptions);
                    builder.UseUrls(url);

                    // This is important to let UseServiceFabricIntegration execute first - otherwise listener.UrlSuffix would be an empty string.
                    var listenerInformation = new ServiceHostAspNetCoreListenerInformation(
                        parameters.EndpointName,
                        listener.UrlSuffix);

                    builder.ConfigureServices(
                        services =>
                        {
                            DependencyRegistrant.Register(services, serviceContext);
                            DependencyRegistrant.Register(services, servicePartition);
                            DependencyRegistrant.Register(services, serviceEventSource);
                            DependencyRegistrant.Register(services, listenerInformation);

                            parameters.DependenciesConfigAction?.Invoke(services);
                        });

                    var loggerOptions = parameters.LoggerOptionsFunc();
                    if (loggerOptions == null)
                    {
                        throw new FactoryProducesNullInstanceException<IServiceHostLoggerOptions>();
                    }

                    builder.ConfigureLogging(
                        config =>
                        {
                            config.AddProvider(new ServiceHostAspNetCoreListenerLoggerProvider(listenerInformation, loggerOptions, serviceEventSource));
                        });

                    return builder.Build();
                });

            return context => parameters.AspNetCoreCommunicationListenerFunc(context, parameters.EndpointName, build);
        }
    }
}