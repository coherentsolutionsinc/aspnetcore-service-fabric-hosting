using System;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.DependencyInjection.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.DependencyInjection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostDelegateReplicaTemplate<TService, TParameters, TConfigurator, TDelegate>
        : ServiceHostBuilderBlock<TParameters, TConfigurator>, IServiceHostDelegateReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostDelegateReplicaTemplateParameters
        where TConfigurator : IServiceHostDelegateReplicaTemplateConfigurator
    {
        protected abstract class DelegateParameters : BlockParameters,
            IServiceHostDelegateReplicaTemplateParameters,
            IServiceHostDelegateReplicaTemplateConfigurator
        {
            [RequiredConfiguration(nameof(UseDelegate))]
            public Delegate Delegate
            {
                get; private set;
            }

            [RequiredConfiguration(nameof(UseDelegateInvoker))]
            public Func<IServiceProvider, IServiceDelegateInvoker> DelegateInvokerFunc
            {
                get; private set;
            }

            protected DelegateParameters()
            {
                this.Delegate = null;
                this.DelegateInvokerFunc = null;
            }

            public void UseDelegate(
                Delegate @delegate)
            {
                this.Delegate = @delegate
                 ?? throw new ArgumentNullException(nameof(@delegate));
            }

            public void UseDelegateInvoker(
                Func<IServiceProvider, IServiceDelegateInvoker> factoryFunc)
            {
                this.DelegateInvokerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }
        }

        public abstract TDelegate Activate(
            TService service);

        protected Func<TService, IServiceDelegateInvoker> CreateFactory(
            TParameters parameters)
        {
            this.ValidateUpstreamConfiguration(parameters);

            var build = new Func<TService, IServiceDelegateInvoker>(
                service =>
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

                    var loggerOptions = parameters.LoggerOptionsFunc();
                    if (loggerOptions is null)
                    {
                        throw new FactoryProducesNullInstanceException<IConfigurableObjectLoggerOptions>();
                    }

                    dependenciesCollection.AddLogging(
                        builder =>
                        {
                            builder.AddProvider(
                                new ServiceHostDelegateLoggerProvider(serviceContext, serviceEventSource, loggerOptions));
                        });

                    // Possible point of proxination
                    parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

                    // Creating implementation with proxination support
                    return parameters.DelegateInvokerFunc(
                        new ProxynatorAwareServiceProvider(dependenciesCollection.BuildServiceProvider()));
                });

            return build;
        }
    }
}