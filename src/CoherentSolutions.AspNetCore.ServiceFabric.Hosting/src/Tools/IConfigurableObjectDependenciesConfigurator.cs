using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools
{
    public interface IConfigurableObjectDependenciesConfigurator
    {
        void ConfigureDependencies(
            Action<IServiceCollection> configAction);
    }
}