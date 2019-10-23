using System;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderBlockParameters :
        IConfigurableObjectDependenciesParameters,
        IConfigurableObjectLoggerParameters
    {
    
    }

    public interface IServiceHostBuilderBlockConfigurator :
        IConfigurableObjectDependenciesConfigurator,
        IConfigurableObjectLoggerConfigurator
    {
    
    }

    public abstract class ServiceHostBuilderBlock<TParameters, TConfigurator>
        : ValidateableConfigurableObject<TParameters, TConfigurator>
        where TParameters : IServiceHostBuilderBlockParameters
        where TConfigurator : IServiceHostBuilderBlockConfigurator
    {
        protected class BlockParameters :
            IServiceHostBuilderBlockParameters,
            IServiceHostBuilderBlockConfigurator
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
    }
}