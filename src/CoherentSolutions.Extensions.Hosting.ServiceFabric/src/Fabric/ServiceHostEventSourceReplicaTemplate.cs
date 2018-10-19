using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.DependencyInjection.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.DependencyInjection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostEventSourceReplicaTemplate<TServiceInformation, TParameters, TConfigurator, TEventSource>
        : ConfigurableObject<TConfigurator>, IServiceHostEventSourceReplicaTemplate<TConfigurator>
        where TServiceInformation : IServiceInformation
        where TParameters : IServiceHostEventSourceReplicaTemplateParameters
        where TConfigurator : IServiceHostEventSourceReplicaTemplateConfigurator
    {
        protected abstract class EventSourceParameters
            : IServiceHostEventSourceReplicaTemplateParameters,
              IServiceHostEventSourceReplicaTemplateConfigurator
        {
            public Func<IServiceProvider, IServiceEventSource> ImplementationFunc { get; private set; }

            public Func<IServiceCollection> DependenciesFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected EventSourceParameters()
            {
                this.ImplementationFunc = DefaultImplementationFunc;
                this.DependenciesFunc = DefaultDependenciesFunc;
                this.DependenciesConfigAction = null;
            }

            public void UseImplementation<TImplementation>(
                Func<IServiceProvider, TImplementation> factoryFunc)
                where TImplementation : IServiceEventSource
            {
                if (factoryFunc == null)
                {
                    throw new ArgumentNullException(nameof(factoryFunc));
                }

                this.ImplementationFunc = provider => factoryFunc(provider);
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

            private static IServiceEventSource DefaultImplementationFunc(
                IServiceProvider serviceProvider)
            {
                return ActivatorUtilities.CreateInstance<ServiceHostEventSource>(serviceProvider);
            }

            private static IServiceCollection DefaultDependenciesFunc()
            {
                return new ServiceCollection();
            }
        }

        public abstract TEventSource Activate(
            TServiceInformation serviceInformation);

        protected Func<TServiceInformation, IServiceEventSource> CreateEventSourceFunc(
            TParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var build = new Func<TServiceInformation, IServiceEventSource>(
                serviceInformation =>
                {
                    var dependenciesCollection = parameters.DependenciesFunc();
                    if (dependenciesCollection == null)
                    {
                        throw new FactoryProducesNullInstanceException<IServiceCollection>();
                    }

                    dependenciesCollection.Add(serviceInformation.GetContext());
                    dependenciesCollection.Add(serviceInformation.GetPartition());

                    // Possible point of proxination
                    parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

                    // Adding support for open-generics
                    var provider = new ProxynatorAwareServiceProvider(dependenciesCollection.BuildServiceProvider());

                    return parameters.ImplementationFunc(provider);
                });

            return build;
        }
    }
}