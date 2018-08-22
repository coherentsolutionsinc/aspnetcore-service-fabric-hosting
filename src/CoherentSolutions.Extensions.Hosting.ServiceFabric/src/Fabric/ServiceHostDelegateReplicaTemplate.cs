using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostDelegateReplicaTemplate<TService, TParameters, TConfigurator, TDelegateInvoker, TDelegate>
        : ConfigurableObject<TConfigurator>, IServiceHostDelegateReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostDelegateReplicaTemplateParameters
        where TConfigurator : IServiceHostDelegateReplicaTemplateConfigurator
    {
        protected abstract class DelegateParameters
            : IServiceHostDelegateReplicaTemplateParameters,
              IServiceHostDelegateReplicaTemplateConfigurator
        {
            public Delegate Delegate { get; private set; }

            public Func<IServiceHostLoggerOptions> LoggerOptionsFunc { get; private set; }

            public Func<IServiceCollection> DependenciesFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected DelegateParameters()
            {
                this.LoggerOptionsFunc = DefaultLoggerOptionsFunc;
                this.Delegate = null;
                this.DependenciesFunc = DefaultDependenciesFunc;
                this.DependenciesConfigAction = null;
            }

            public void UseDelegate(
                Delegate @delegate)
            {
                this.Delegate = @delegate
                 ?? throw new ArgumentNullException(nameof(@delegate));
            }

            public void UseLoggerOptions(
                Func<IServiceHostLoggerOptions> factoryFunc)
            {
                this.LoggerOptionsFunc = factoryFunc
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

            private static IServiceHostLoggerOptions DefaultLoggerOptionsFunc()
            {
                return ServiceHostLoggerOptions.Disabled;
            }

            private static IServiceCollection DefaultDependenciesFunc()
            {
                return new ServiceCollection();
            }
        }

        public abstract TDelegate Activate(
            TService service);

        protected Func<Func<Delegate, IServiceProvider, TDelegateInvoker>, TDelegateInvoker> CreateDelegateInvokerFunc(
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

            var build = new Func<Func<Delegate, IServiceProvider, TDelegateInvoker>, TDelegateInvoker>(
                factory =>
                {
                    var serviceContext = service.GetContext();
                    var servicePartition = service.GetPartition();
                    var serviceEventSource = service.GetEventSource();

                    var dependenciesCollection = parameters.DependenciesFunc();
                    if (dependenciesCollection == null)
                    {
                        throw new FactoryProducesNullInstanceException<IServiceCollection>();
                    }

                    dependenciesCollection.Add(serviceContext);
                    dependenciesCollection.Add(servicePartition);
                    dependenciesCollection.Add(serviceEventSource);

                    parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

                    var loggerOptions = parameters.LoggerOptionsFunc();
                    if (loggerOptions == null)
                    {
                        throw new FactoryProducesNullInstanceException<IServiceHostLoggerOptions>();
                    }

                    dependenciesCollection.AddLogging(
                        builder =>
                        {
                            builder.AddProvider(new ServiceHostDelegateLoggerProvider(loggerOptions, serviceEventSource));
                        });

                    // Adding support for open-generics
                    var provider = new ProxynatorAwareServiceProvider(dependenciesCollection.BuildServiceProvider());

                    var invoker = factory(parameters.Delegate, provider);
                    if (invoker == null)
                    {
                        throw new FactoryProducesNullInstanceException<TDelegateInvoker>();
                    }

                    return invoker;
                });

            return build;
        }
    }
}