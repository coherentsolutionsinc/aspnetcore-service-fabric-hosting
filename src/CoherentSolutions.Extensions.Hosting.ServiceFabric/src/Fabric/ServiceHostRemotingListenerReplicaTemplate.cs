using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

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

            public Func<IServiceProvider, IRemotingImplementation> RemotingImplementationFunc { get; private set; }

            public Func<FabricTransportRemotingListenerSettings> RemotingSettingsFunc { get; private set; }

            public Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> RemotingSerializerFunc { get; private set; }

            public Func<IServiceProvider, IServiceRemotingMessageHandler> RemotingHandlerFunc { get; private set; }

            protected RemotingListenerParameters()
            {
                this.RemotingCommunicationListenerFunc = DefaultRemotingCommunicationListenerFunc;
                this.RemotingImplementationFunc = null;
                this.RemotingSettingsFunc = DefaultRemotingSettingsFunc;
                this.RemotingSerializerFunc = null;
                this.RemotingHandlerFunc = DefaultRemotingHandlerFunc;
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
                this.RemotingImplementationFunc = factoryFunc == null
                    ? provider => ActivatorUtilities.CreateInstance<TImplementation>(provider)
                    : (Func<IServiceProvider, IRemotingImplementation>) (provider => factoryFunc(provider));
            }

            public void UseSettings(
                Func<FabricTransportRemotingListenerSettings> factoryFunc)
            {
                this.RemotingSettingsFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseSerializer<TSerializer>(
                Func<IServiceProvider, TSerializer> factoryFunc)
                where TSerializer : IServiceRemotingMessageSerializationProvider
            {
                this.RemotingSerializerFunc = factoryFunc == null
                    ? provider => ActivatorUtilities.CreateInstance<TSerializer>(provider)
                    : (Func<IServiceProvider, IServiceRemotingMessageSerializationProvider>) (provider => factoryFunc(provider));
            }

            public void UseHandler<THandler>(
                Func<IServiceProvider, THandler> factoryFunc)
                where THandler : IServiceRemotingMessageHandler
            {
                this.RemotingHandlerFunc = factoryFunc == null
                    ? provider => ActivatorUtilities.CreateInstance<THandler>(provider)
                    : (Func<IServiceProvider, IServiceRemotingMessageHandler>) (provider => factoryFunc(provider));
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

        protected override Func<ServiceContext, ICommunicationListener> CreateCommunicationListenerFunc(
            TService service,
            TParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.RemotingImplementationFunc == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(parameters.RemotingImplementationFunc)} was configured");
            }

            var listenerInformation = new ServiceHostRemotingListenerInformation(parameters.EndpointName);

            var build = new ServiceHostRemotingCommunicationListenerComponentsFactory(
                context =>
                {
                    var serviceContext = service.GetContext();
                    var servicePartition = service.GetPartition();
                    var serviceEventSource = service.GetEventSource();

                    var dependenciesCollection = parameters.DependenciesFunc();
                    if (dependenciesCollection == null)
                    {
                        throw new FactoryProducesNullInstanceException<IServiceCollection>();
                    }

                    DependencyRegistrant.Register(dependenciesCollection, serviceContext);
                    DependencyRegistrant.Register(dependenciesCollection, servicePartition);
                    DependencyRegistrant.Register(dependenciesCollection, serviceEventSource);
                    DependencyRegistrant.Register(dependenciesCollection, listenerInformation);

                    parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

                    var loggerOptions = parameters.LoggerOptionsFunc();
                    if (loggerOptions == null)
                    {
                        throw new FactoryProducesNullInstanceException<IServiceHostLoggerOptions>();
                    }

                    dependenciesCollection.AddLogging(
                        builder =>
                        {
                            builder.AddProvider(new ServiceHostRemotingListenerLoggerProvider(listenerInformation, loggerOptions, serviceEventSource));
                        });

                    var provider = dependenciesCollection.BuildServiceProvider();

                    var implementation = parameters.RemotingImplementationFunc(provider);
                    if (implementation == null)
                    {
                        throw new FactoryProducesNullInstanceException<IRemotingImplementation>();
                    }

                    var serializer = (IServiceRemotingMessageSerializationProvider) null;
                    if (parameters.RemotingSerializerFunc != null)
                    {
                        serializer = parameters.RemotingSerializerFunc(provider);
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

                    var implementationType = implementation.GetType();
                    var overrides = new Dictionary<Type, object>
                    {
                        [implementationType] = implementation
                    };
                    foreach (var @interface in implementationType
                       .GetInterfaces()
                       .Where(i => typeof(IRemotingImplementation).IsAssignableFrom(i)))
                    {
                        overrides[@interface] = implementation;
                    }

                    var logger = (ILogger) provider.GetService(typeof(ILogger<>).MakeGenericType(implementation.GetType()));
                    var handler = parameters.RemotingHandlerFunc(new OverridableServiceProvider(overrides, provider));

                    return new ServiceHostRemotingCommunicationListenerComponents(
                        new ServiceHostRemotingListenerMessageHandler(handler, logger), 
                        serializer, 
                        settings);
                });

            return context => parameters.RemotingCommunicationListenerFunc(context, build);
        }
    }
}