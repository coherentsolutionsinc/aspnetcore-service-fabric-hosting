using System;
using System.Fabric;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common.Exceptions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;

using IRemotingImplementation = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
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
            public Func<IServiceProvider, IRemotingImplementation> RemotingImplementationFunc { get; private set; }

            public Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> RemotingSerializerFunc { get; private set; }

            protected RemotingListenerParameters()
            {
                this.RemotingImplementationFunc = null;
                this.RemotingSerializerFunc = null;
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

            var build = new Func<ServiceContext, FabricTransportServiceRemotingListener>(
                context =>
                {
                    var serviceContext = service.GetContext();
                    var servicePartition = service.GetPartition();
                    var serviceEventSource = service.GetEventSource();

                    var services = new ServiceCollection();

                    ServiceHostDependencyRegistrant.Register(services, serviceContext);
                    ServiceHostDependencyRegistrant.Register(services, servicePartition);
                    ServiceHostDependencyRegistrant.Register(services, serviceEventSource);
                    ServiceHostDependencyRegistrant.Register(services, listenerInformation);

                    var loggerOptions = parameters.LoggerOptionsFunc();
                    if (loggerOptions == null)
                    {
                        throw new FactoryProducesNullInstanceException<IServiceHostListenerLoggerOptions>();
                    }

                    services.AddLogging(
                        builder =>
                        {
                            builder.AddProvider(new ServiceHostRemotingListenerLoggerProvider(listenerInformation, loggerOptions, serviceEventSource));
                        });

                    var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

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

                    return new FabricTransportServiceRemotingListener(context, handler, settings, serializer);
                });

            return context => build(context);
        }
    }
}