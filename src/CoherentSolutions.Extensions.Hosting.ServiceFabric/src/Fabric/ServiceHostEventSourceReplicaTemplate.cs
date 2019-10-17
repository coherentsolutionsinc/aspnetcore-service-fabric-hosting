using System;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.DependencyInjection.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.DependencyInjection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostEventSourceReplicaTemplate<TServiceInformation, TParameters, TConfigurator, TEventSource>
        : ValidateableConfigurableObject<TParameters, TConfigurator>, IServiceHostEventSourceReplicaTemplate<TConfigurator>
        where TServiceInformation : IServiceInformation
        where TParameters : IServiceHostEventSourceReplicaTemplateParameters
        where TConfigurator : IServiceHostEventSourceReplicaTemplateConfigurator
    {
        protected abstract class EventSourceParameters
            : IServiceHostEventSourceReplicaTemplateParameters,
              IServiceHostEventSourceReplicaTemplateConfigurator
        {
            [RequiredConfiguration(nameof(UseImplementation))]
            public Func<IServiceProvider, IServiceEventSource> ImplementationFunc
            {
                get; private set;
            }

            [RequiredConfiguration(nameof(UseDependencies))]
            public Func<IServiceCollection> DependenciesFunc
            {
                get; private set;
            }

            public Action<IServiceCollection> DependenciesConfigAction
            {
                get; private set;
            }

            protected EventSourceParameters()
            {
                this.ImplementationFunc = null;
                this.DependenciesFunc = null;
                this.DependenciesConfigAction = null;
            }

            public void UseImplementation(
                Func<IServiceProvider, IServiceEventSource> factoryFunc)
            {
                this.ImplementationFunc = factoryFunc
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
                if (configAction is null)
                {
                    throw new ArgumentNullException(nameof(configAction));
                }

                this.DependenciesConfigAction = this.DependenciesConfigAction.Chain(configAction);
            }
        }

        public abstract TEventSource Activate(
            TServiceInformation serviceInformation);

        protected Func<TServiceInformation, IServiceEventSource> CreateFactory(
            TParameters parameters)
        {
            this.ValidateUpstreamConfiguration(parameters);

            var build = new Func<TServiceInformation, IServiceEventSource>(
                serviceInformation =>
                {
                    var dependenciesCollection = parameters.DependenciesFunc();
                    if (dependenciesCollection is null)
                    {
                        throw new FactoryProducesNullInstanceException<IServiceCollection>();
                    }

                    dependenciesCollection.Add(serviceInformation.GetContext());
                    dependenciesCollection.Add(serviceInformation.GetPartition());

                    // Possible point of proxination
                    parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

                    // Creating implementation with proxination support
                    return parameters.ImplementationFunc(
                        new ProxynatorAwareServiceProvider(dependenciesCollection.BuildServiceProvider()));
                });

            return build;
        }
    }
}