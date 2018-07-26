using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class ConfigureDependenciesTheoryExtension : IConfigureDependenciesTheoryExtension
    {
        public Action<IServiceCollection> ConfigAction { get; private set; }

        public ConfigureDependenciesTheoryExtension()
        {
            this.ConfigAction = services =>
            {
            };
        }

        public ConfigureDependenciesTheoryExtension Setup(
            Action<IServiceCollection> configAction)
        {
            this.ConfigAction = configAction;

            return this;
        }
    }
}