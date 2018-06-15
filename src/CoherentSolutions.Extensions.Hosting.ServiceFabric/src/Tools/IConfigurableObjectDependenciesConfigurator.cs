using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public interface IConfigurableObjectDependenciesConfigurator
    {
        void ConfigureDependencies(
            Action<IServiceCollection> configAction);
    }
}