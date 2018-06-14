using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static TCaller ConfigureDependencies<TCaller>(
            this TCaller @this,
            Action<IServiceCollection> configAction)
            where TCaller : IConfigurableObject<IConfigurableObjectDependenciesConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureDependencies(configAction));

            return @this;
        }
    }
}