﻿using System;
using System.Collections.Generic;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.DependencyInjection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.DependencyInjection.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.DependencyInjection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

using IRemotingImplementation = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostRemotingListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        : ServiceHostListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        where TService : IService
        where TParameters : IServiceHostRemotingListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostRemotingListenerReplicaTemplateConfigurator
    {
        protected abstract class RemotingListenerParameters
            : ListenerParameters,
              IServiceHostRemotingListenerReplicaTemplateParameters,
              IServiceHostRemotingListenerReplicaTemplateConfigurator
        {
            public ServiceHostRemotingCommunicationListenerFactory RemotingCommunicationListenerFunc { get; private set; }

            [RequiredConfiguration(nameof(UseImplementation))]
            public Func<IServiceProvider, IRemotingImplementation> RemotingImplementationFunc { get; private set; }

            public Func<FabricTransportRemotingListenerSettings> RemotingSettingsFunc { get; private set; }

            public Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> RemotingSerializationProviderFunc { get; private set; }

            public Func<IServiceProvider, IServiceRemotingMessageHandler> RemotingHandlerFunc { get; private set; }

            [RequiredConfiguration(nameof(UseDependencies))]
            public Func<IServiceCollection> DependenciesFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            [RequiredConfiguration(nameof(UseLoggerOptions))]
            public Func<IConfigurableObjectLoggerOptions> LoggerOptionsFunc { get; private set; }

            protected RemotingListenerParameters()
            {
                this.RemotingCommunicationListenerFunc = DefaultRemotingCommunicationListenerFunc;
                this.RemotingImplementationFunc = null;
                this.RemotingSettingsFunc = DefaultRemotingSettingsFunc;
                this.RemotingSerializationProviderFunc = null;
                this.RemotingHandlerFunc = DefaultRemotingHandlerFunc;
                this.DependenciesFunc = null;
                this.DependenciesConfigAction = null;
                this.LoggerOptionsFunc = null;
            }

            public void UseCommunicationListener(
                ServiceHostRemotingCommunicationListenerFactory factoryFunc)
            {
                this.RemotingCommunicationListenerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseImplementation<TImplementation>(
                Func<IServiceProvider, TImplementation> factoryFunc)
                where TImplementation : IRemotingImplementation
            {
                if (factoryFunc is null)
                {
                    throw new ArgumentNullException(nameof(factoryFunc));
                }

                this.RemotingImplementationFunc = provider => factoryFunc(provider);
            }

            public void UseSettings(
                Func<FabricTransportRemotingListenerSettings> factoryFunc)
            {
                this.RemotingSettingsFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseSerializationProvider<TSerializationProvider>(
                Func<IServiceProvider, TSerializationProvider> factoryFunc)
                where TSerializationProvider : IServiceRemotingMessageSerializationProvider
            {
                if (factoryFunc is null)
                {
                    throw new ArgumentNullException(nameof(factoryFunc));
                }

                this.RemotingSerializationProviderFunc = provider => factoryFunc(provider);
            }

            public void UseHandler<THandler>(
                Func<IServiceProvider, THandler> factoryFunc)
                where THandler : IServiceRemotingMessageHandler
            {
                if (factoryFunc is null)
                {
                    throw new ArgumentNullException(nameof(factoryFunc));
                }

                this.RemotingHandlerFunc = provider => factoryFunc(provider);
            }

            public void UseDependencies(
                Func<IServiceCollection> factoryFunc)
            {
                this.DependenciesFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void ConfigureDependencies(
                Action<IServiceCollection> configAction)
            {
                if (configAction is null)
                {
                    throw new ArgumentNullException(nameof(configAction));
                }

                this.DependenciesConfigAction = this.DependenciesConfigAction.Chain(configAction);
            }

            public void UseLoggerOptions(
                Func<IConfigurableObjectLoggerOptions> factoryFunc)
            {
                this.LoggerOptionsFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            private static FabricTransportServiceRemotingListener DefaultRemotingCommunicationListenerFunc(
                ServiceContext serviceContext,
                ServiceHostRemotingCommunicationListenerComponentsFactory build)
            {
                var components = build(serviceContext);
                return new FabricTransportServiceRemotingListener(
                    serviceContext,
                    components.MessageHandler,
                    components.ListenerSettings,
                    components.MessageSerializationProvider);
            }

            private static FabricTransportRemotingListenerSettings DefaultRemotingSettingsFunc()
            {
                return new FabricTransportRemotingListenerSettings();
            }

            private static IServiceRemotingMessageHandler DefaultRemotingHandlerFunc(
                IServiceProvider provider)
            {
                var serviceContext = provider.GetService<ServiceContext>();
                var serviceImplementation = provider.GetService<IRemotingImplementation>();
                var serviceRemotingMessageBodyFactory = provider.GetService<IServiceRemotingMessageBodyFactory>();

                return new ServiceRemotingMessageDispatcher(
                    serviceContext,
                    serviceImplementation,
                    serviceRemotingMessageBodyFactory);
            }
        }

        protected override Func<TService, ICommunicationListener> CreateFactory(
            TParameters parameters)
        {
            this.ValidateUpstreamConfiguration(parameters);

            return service =>
            {
                var listenerInformation = new ServiceHostRemotingListenerInformation(parameters.EndpointName);

                var build = new ServiceHostRemotingCommunicationListenerComponentsFactory(
                    context =>
                    {
                        var serviceContext = service.GetContext();
                        var servicePartition = service.GetPartition();
                        var serviceEventSource = service.GetEventSource();

                        var dependenciesCollection = parameters.DependenciesFunc();
                        if (dependenciesCollection is null)
                        {
                            throw new FactoryProducesNullInstanceException<IServiceCollection>();
                        }

                        // We need register all level dependencies first in order to make
                        // sure that no level dependencies will be ignore during proxination
                        dependenciesCollection.Add(serviceContext);
                        dependenciesCollection.Add(servicePartition);
                        dependenciesCollection.Add(serviceEventSource);
                        dependenciesCollection.Add(listenerInformation);

                        var loggerOptions = parameters.LoggerOptionsFunc();
                        if (loggerOptions is null)
                        {
                            throw new FactoryProducesNullInstanceException<IConfigurableObjectLoggerOptions>();
                        }

                        dependenciesCollection.AddLogging(
                            builder =>
                            {
                                builder.AddProvider(
                                    new ServiceHostRemotingListenerLoggerProvider(
                                        listenerInformation,
                                        serviceContext,
                                        serviceEventSource,
                                        loggerOptions));
                            });

                        // Possible point of proxination
                        parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

                        // Adding open-generic proxies
                        IServiceProvider provider = new ProxynatorAwareServiceProvider(dependenciesCollection.BuildServiceProvider());

                        var implementation = parameters.RemotingImplementationFunc(provider);
                        if (implementation is null)
                        {
                            throw new FactoryProducesNullInstanceException<IRemotingImplementation>();
                        }

                        var implementationType = implementation.GetType();
                        var replacements = new Dictionary<Type, object>
                        {
                            [implementationType] = implementation,
                            [typeof(IRemotingImplementation)] = implementation
                        };

                        // Adding implementation as singleton
                        provider = new ReplaceAwareServiceProvider(replacements, provider);

                        var serializer = (IServiceRemotingMessageSerializationProvider)null;
                        if (parameters.RemotingSerializationProviderFunc != null)
                        {
                            serializer = parameters.RemotingSerializationProviderFunc(provider);
                            if (serializer == null)
                            {
                                throw new FactoryProducesNullInstanceException<IServiceRemotingMessageSerializationProvider>();
                            }
                        }

                        var settings = parameters.RemotingSettingsFunc();
                        if (settings == null)
                        {
                            throw new FactoryProducesNullInstanceException<FabricTransportRemotingListenerSettings>();
                        }

                        settings.EndpointResourceName = parameters.EndpointName;

                        var logger = (ILogger)provider.GetService(typeof(ILogger<>).MakeGenericType(implementation.GetType()));
                        var handler = parameters.RemotingHandlerFunc(provider);

                        return new ServiceHostRemotingCommunicationListenerComponents(
                            new ServiceHostRemotingListenerMessageHandler(handler, logger),
                            serializer,
                            settings);
                    });

                return parameters.RemotingCommunicationListenerFunc(service.GetContext(), build);
            };
        }
    }
}