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
        : ServiceHostReplicaTemplate<TServiceInformation, TEventSource, TParameters, TConfigurator>, IServiceHostEventSourceReplicaTemplate<TConfigurator>
        where TServiceInformation : IServiceInformation
        where TParameters : IServiceHostEventSourceReplicaTemplateParameters
        where TConfigurator : IServiceHostEventSourceReplicaTemplateConfigurator
    {
        protected abstract class EventSourceParameters : ReplicaTemplateParameters,
            IServiceHostEventSourceReplicaTemplateParameters,
            IServiceHostEventSourceReplicaTemplateConfigurator
        {
            [RequiredConfiguration(nameof(UseImplementation))]
            public Func<IServiceProvider, IServiceEventSource> ImplementationFunc
            {
                get; private set;
            }

            protected EventSourceParameters()
            {
                this.ImplementationFunc = null;
            }
            public void UseImplementation(
                Func<IServiceProvider, IServiceEventSource> factoryFunc)
            {
                this.ImplementationFunc = factoryFunc
                    ?? throw new ArgumentNullException(nameof(factoryFunc));
            }
        }

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