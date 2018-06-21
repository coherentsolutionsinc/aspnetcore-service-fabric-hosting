using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

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
            public Delegate Delegate { get; private set; }

            public ServiceLifecycleEvent LifecycleEvent { get; private set; }

            public Func<IServiceCollection> DependenciesFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected DelegateParameters()
            {
                this.Delegate = null;
                this.LifecycleEvent = ServiceLifecycleEvent.Unknown;
                this.DependenciesFunc = DefaulDependenciesFunc;
                this.DependenciesConfigAction = null;
            }

            public void UseDelegate(
                Delegate @delegate)
            {
                this.Delegate = @delegate
                 ?? throw new ArgumentNullException(nameof(@delegate));
            }

            public void UseLifecycleEvent(
                ServiceLifecycleEvent lifecycleEvent)
            {
                this.LifecycleEvent = lifecycleEvent;
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

            public static IServiceCollection DefaulDependenciesFunc()
            {
                return new ServiceCollection();
            }
        }

        public abstract TDelegate Activate(
            TService service);

        protected Func<IServiceHostDelegate> CreateFunc(
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

            if (parameters.LifecycleEvent == ServiceLifecycleEvent.Unknown)
            {
                throw new ArgumentException("No service life-cycle event set for delegate.");
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

            var provider = dependenciesCollection.BuildServiceProvider();

            return () => new ServiceHostDelegate(parameters.Delegate, parameters.LifecycleEvent, provider);
        }
    }
}