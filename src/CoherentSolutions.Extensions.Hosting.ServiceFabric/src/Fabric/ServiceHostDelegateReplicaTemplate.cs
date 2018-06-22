using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostDelegateReplicaTemplate<TService, TParameters, TConfigurator, TDelegate>
        : ConfigurableObject<TConfigurator>, IServiceHostDelegateReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostDelegateReplicaTemplateParameters
        where TConfigurator : IServiceHostDelegateReplicaTemplateConfigurator
    {
        protected abstract class DelegateParameters
            : IServiceHostDelegateReplicaTemplateParameters,
              IServiceHostDelegateReplicaTemplateConfigurator
        {
            public Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker> DelegateInvokerFunc { get; private set; }

            public Delegate Delegate { get; private set; }

            public Func<IServiceHostLoggerOptions> LoggerOptionsFunc { get; private set; }

            public Func<IServiceCollection> DependenciesFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected DelegateParameters()
            {
                this.LoggerOptionsFunc = DefaultLoggerOptionsFunc;
                this.DelegateInvokerFunc = DefaulDelegateInvokerFunc;
                this.Delegate = null;
                this.DependenciesFunc = DefaulDependenciesFunc;
                this.DependenciesConfigAction = null;
            }

            public void UseDelegateInvoker(
                Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker> factoryFunc)
            {
                this.DelegateInvokerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
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

            private static IServiceHostDelegateInvoker DefaulDelegateInvokerFunc(
                Delegate @delegate,
                IServiceProvider services)
            {
                if (@delegate == null)
                {
                    throw new ArgumentNullException(nameof(@delegate));
                }

                if (services == null)
                {
                    throw new ArgumentNullException(nameof(services));
                }

                return new ServiceHostDelegateInvoker(@delegate, services);
            }

            private static IServiceHostLoggerOptions DefaultLoggerOptionsFunc()
            {
                return ServiceHostLoggerOptions.Disabled;
            }

            private static IServiceCollection DefaulDependenciesFunc()
            {
                return new ServiceCollection();
            }
        }

        public abstract TDelegate Activate(
            TService service);

        protected Func<IServiceHostDelegateInvoker> CreateFunc(
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

            var delegateInvokerFunc = parameters.DelegateInvokerFunc;
            if (delegateInvokerFunc == null)
            {
                throw new InvalidOperationException(
                    $"No {parameters.DelegateInvokerFunc} was configured.");
            }

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

            var provider = dependenciesCollection.BuildServiceProvider();

            return () =>
            {
                var invoker = delegateInvokerFunc(parameters.Delegate, provider);
                if (invoker == null)
                {
                    throw new FactoryProducesNullInstanceException<IServiceHostDelegateInvoker>();
                }
                return invoker;
            };
        }
    }
}