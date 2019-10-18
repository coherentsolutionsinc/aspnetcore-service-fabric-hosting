using System;
using System.Fabric;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.DependencyInjection.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.AspNetCore;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            public ServiceFabricIntegrationOptions IntegrationOptions
            {
                get; private set;
            }

            [RequiredConfiguration(nameof(UseWebHostBuilder))]
            public Func<IWebHostBuilder> WebHostBuilderFunc
            {
                get; private set;
            }

            public Action<IWebHostBuilder> WebHostConfigAction
            {
                get; private set;
            }

            [RequiredConfiguration(nameof(UseCommunicationListener))]
            public ServiceHostAspNetCoreCommunicationListenerFactory AspNetCoreCommunicationListenerFunc
            {
                get; private set;
            }

            [RequiredConfiguration(nameof(UseCommunicationListener))]
            public Action<IWebHostBuilder> WebHostCommunicationListenerConfigAction
            {
                get; private set;
            }

            protected AspNetCoreListenerParameters()
            {
                this.IntegrationOptions = ServiceFabricIntegrationOptions.None;
                this.AspNetCoreCommunicationListenerFunc = null;
                this.WebHostBuilderFunc = null;
                this.WebHostConfigAction = null;
                this.WebHostCommunicationListenerConfigAction = null;
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
                ServiceHostAspNetCoreCommunicationListenerFactory factoryFunc,
                Action<IWebHostBuilder> configAction)
            {
                this.AspNetCoreCommunicationListenerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));

                this.WebHostCommunicationListenerConfigAction = configAction
                 ?? throw new ArgumentNullException(nameof(configAction));
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
        }

        protected override Func<TService, ICommunicationListener> CreateFactory(
            TParameters parameters)
        {
            this.ValidateUpstreamConfiguration(parameters);

            return new Func<TService, ICommunicationListener>(
                service =>
                {
                    var build = new Func<string, AspNetCoreCommunicationListener, IWebHost>(
                        (url, listener) =>
                        {
                            var serviceContext = service.GetContext();
                            var servicePartition = service.GetPartition();
                            var serviceEventSource = service.GetEventSource();

                            var builder = parameters.WebHostBuilderFunc();
                            if (builder is null)
                            {
                                throw new FactoryProducesNullInstanceException<IWebHostBuilder>();
                            }

                            parameters.WebHostConfigAction(builder);
                            parameters.WebHostCommunicationListenerConfigAction(builder);

                            builder.UseServiceFabricIntegration(listener, parameters.IntegrationOptions);
                            builder.UseUrls(url);

                            // This is important to let UseServiceFabricIntegration execute first - otherwise listener.UrlSuffix would be an empty string.
                            var listenerInformation = new ServiceHostAspNetCoreListenerInformation(
                                parameters.EndpointName,
                                listener.UrlSuffix);

                            builder.ConfigureServices(
                                services =>
                                {
                                    // We need register all level dependencies first in order to make
                                    // sure that no level dependencies will be ignore during proxination
                                    services.Add(serviceContext);
                                    services.Add(servicePartition);
                                    services.Add(serviceEventSource);
                                    services.Add(listenerInformation);

                                    var loggerOptions = parameters.LoggerOptionsFunc();
                                    if (loggerOptions is null)
                                    {
                                        throw new FactoryProducesNullInstanceException<IConfigurableObjectLoggerOptions>();
                                    }

                                    services.AddLogging(
                                        config =>
                                        {
                                            config.AddProvider(
                                                new ServiceHostAspNetCoreListenerLoggerProvider(
                                                    listenerInformation,
                                                    serviceContext,
                                                    serviceEventSource,
                                                    loggerOptions));
                                        });

                                    var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IStartup));
                                    if (descriptor is object)
                                    {
                                        var replacement = descriptor switch
                                        {
                                            _ when descriptor.ImplementationFactory is object => new ServiceDescriptor(
                                                typeof(IStartup),
                                                provider =>
                                                {
                                                    var impl = descriptor.ImplementationFactory(provider);
                                                    return new ProxynatorAwareStartup((IStartup)impl);
                                                },
                                                ServiceLifetime.Singleton),
                                            _ when descriptor.ImplementationInstance is object => new ServiceDescriptor(
                                                typeof(IStartup),
                                                new ProxynatorAwareStartup((IStartup)descriptor.ImplementationInstance)),
                                            _ => new ServiceDescriptor(
                                                typeof(IStartup),
                                                provider =>
                                                {
                                                    var impl = ActivatorUtilities.CreateInstance(provider, descriptor.ImplementationType);
                                                    return new ProxynatorAwareStartup((IStartup)impl);
                                                },
                                                ServiceLifetime.Singleton),
                                        };
                                        services.Replace(replacement);
                                    }
                                });

                            return builder.Build();
                        });

                    return parameters.AspNetCoreCommunicationListenerFunc(service.GetContext(), parameters.EndpointName, build);
                });
        }
    }
}