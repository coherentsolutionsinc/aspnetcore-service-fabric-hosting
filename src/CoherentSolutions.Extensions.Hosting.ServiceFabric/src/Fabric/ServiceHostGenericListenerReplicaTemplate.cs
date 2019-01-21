using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.DependencyInjection.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.DependencyInjection;
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
            public ServiceHostGenericCommunicationListenerFactory GenericCommunicationListenerFunc { get; private set; }

            public Func<IServiceCollection> DependenciesFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            public Func<IConfigurableObjectLoggerOptions> LoggerOptionsFunc { get; private set; }

            protected GenericListenerParameters()
            {
                this.GenericCommunicationListenerFunc = null;
                this.DependenciesFunc = DefaultDependenciesFunc;
                this.DependenciesConfigAction = null;
                this.LoggerOptionsFunc = DefaultLoggerOptionsFunc;
            }

            public void UseCommunicationListener(
                ServiceHostGenericCommunicationListenerFactory factoryFunc)
            {
                this.GenericCommunicationListenerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
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
                if (configAction == null)
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

            private static IServiceCollection DefaultDependenciesFunc()
            {
                return new ServiceCollection();
            }

            private static IConfigurableObjectLoggerOptions DefaultLoggerOptionsFunc()
            {
                return ServiceHostLoggerOptions.Disabled;
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

            if (parameters.GenericCommunicationListenerFunc == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(parameters.GenericCommunicationListenerFunc)} was configured");
            }

            var listenerInformation = new ServiceHostGenericListenerInformation(parameters.EndpointName);
            return context =>
            {
                var serviceContext = service.GetContext();
                var servicePartition = service.GetPartition();
                var serviceEventSource = service.GetEventSource();

                var dependenciesCollection = parameters.DependenciesFunc();
                if (dependenciesCollection == null)
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
                if (loggerOptions == null)
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
                IServiceProvider provider = new ProxynatorAwareServiceProvider(dependenciesCollection.BuildServiceProvider());

                // Create instance of ICommunicationListener
                return parameters.GenericCommunicationListenerFunc(context, parameters.EndpointName, provider);
            };
        }
    }
}