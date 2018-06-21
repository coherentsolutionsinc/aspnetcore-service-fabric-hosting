using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public interface IConfigurableObjectDependenciesConfigurator
    {
        void UseDependencies(
            Func<IServiceCollection> factoryFunc);

        void ConfigureDependencies(
            Action<IServiceCollection> configAction);
    }
}