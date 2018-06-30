using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;

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

            public Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> RemotingSerializerFunc { get; private set; }

            protected RemotingListenerParameters()
            {
                this.RemotingCommunicationListenerFunc = DefaultRemotingCommunicationListenerFunc;
                this.RemotingImplementationFunc = null;
                this.RemotingSerializerFunc = null;
            }

            public void UseCommunicationListener(
                ServiceHostRemotingCommunicationListenerFactory factoryFunc)
            {
                this.RemotingCommunicationListenerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseImplementation<TImplementation>(
                Func<TImplementation> factoryFunc)
                where TImplementation : IRemotingImplementation
            {
                this.RemotingImplementationFunc = factoryFunc == null
                    ? provider => ActivatorUtilities.CreateInstance<TImplementation>(provider)
                    : (Func<IServiceProvider, IRemotingImplementation>) (provider => factoryFunc());
            }

            public void UseSerializer<TSerializer>(
                Func<TSerializer> factoryFunc)
                where TSerializer : IServiceRemotingMessageSerializationProvider
            {
                this.RemotingSerializerFunc = factoryFunc == null
                    ? provider => ActivatorUtilities.CreateInstance<TSerializer>(provider)
                    : (Func<IServiceProvider, IServiceRemotingMessageSerializationProvider>) (provider => factoryFunc());
            }

            private static FabricTransportServiceRemotingListener DefaultRemotingCommunicationListenerFunc(
                ServiceContext serviceContext,
                ServiceHostRemotingCommunicationListenerComponentsFactory build)
            {
                var components = build(serviceContext);
                return new FabricTransportServiceRemotingListener(
                    serviceContext,
                    components.MessageDispatcher,
                    components.ListenerSettings,
                    components.MessageSerializationProvider);
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

                    var settings = new FabricTransportRemotingListenerSettings
                    {
                        EndpointResourceName = parameters.EndpointName
                    };

                    var logger = (ILogger) provider.GetService(typeof(ILogger<>).MakeGenericType(implementation.GetType()));
                    var handler = new ServiceHostRemotingListenerLoggerMessageHandler(serviceContext, implementation, logger);

                    return new ServiceHostRemotingCommunicationListenerComponents(handler, serializer, settings);
                });

            return context => parameters.RemotingCommunicationListenerFunc(context, build);
        }
    }
}