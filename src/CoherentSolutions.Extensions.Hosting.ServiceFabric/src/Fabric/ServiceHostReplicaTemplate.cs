using System;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostReplicaTemplate<TInput, TOutput, TParameters, TConfigurator>
        : ValidateableConfigurableObject<TParameters, TConfigurator>
        where TParameters : IServiceHostReplicaTemplateParameters
        where TConfigurator : IServiceHostReplicaTemplateConfigurator
    {
        protected class ReplicaTemplateParameters :
            IServiceHostReplicaTemplateParameters,
            IServiceHostReplicaTemplateConfigurator
        {
            [RequiredConfiguration(nameof(UseDependencies))]
            public Func<IServiceCollection> DependenciesFunc
            {
                get; private set;
            }

            public Action<IServiceCollection> DependenciesConfigAction
            {
                get; private set;
            }

            [RequiredConfiguration(nameof(UseLoggerOptions))]
            public Func<IConfigurableObjectLoggerOptions> LoggerOptionsFunc
            {
                get; private set;
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

            public void UseLoggerOptions(
                Func<IConfigurableObjectLoggerOptions> factoryFunc)
            {
                this.LoggerOptionsFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }
        }

        public abstract TOutput Activate(
            TInput service);
    }
}