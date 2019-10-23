using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.DependencyInjection.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.DependencyInjection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostGenericListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        : ServiceHostListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        where TService : IService
        where TParameters : IServiceHostGenericListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostGenericListenerReplicaTemplateConfigurator
    {
        protected abstract class GenericListenerParameters
            : ListenerParameters,
              IServiceHostGenericListenerReplicaTemplateParameters,
              IServiceHostGenericListenerReplicaTemplateConfigurator
        {
            [RequiredConfiguration(nameof(UseCommunicationListener))]
            public ServiceHostGenericCommunicationListenerFactory GenericCommunicationListenerFunc { get; private set; }

            protected GenericListenerParameters()
            {
                this.GenericCommunicationListenerFunc = null;
            }

            public void UseCommunicationListener(
                ServiceHostGenericCommunicationListenerFactory factoryFunc)
            {
                this.GenericCommunicationListenerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }
        }

        protected override Func<TService, ICommunicationListener> CreateFactory(
            TParameters parameters)
        {
            this.ValidateUpstreamConfiguration(parameters);

            return service =>
            {
                var serviceContext = service.GetContext();
                var servicePartition = service.GetPartition();
                var serviceEventSource = service.GetEventSource();

                var listenerInformation = new ServiceHostGenericListenerInformation(parameters.EndpointName);

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
                            new ServiceHostGenericListenerLoggerProvider(
                                listenerInformation,
                                serviceContext,
                                serviceEventSource,
                                loggerOptions));
                    });

                // Possible point of proxination
                parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

                // Adding open-generic proxies
                var provider = new ProxynatorAwareServiceProvider(
                    dependenciesCollection.BuildServiceProvider());

                // Create instance of ICommunicationListener
                return parameters.GenericCommunicationListenerFunc(service.GetContext(), parameters.EndpointName, provider);
            };
        }
    }
}