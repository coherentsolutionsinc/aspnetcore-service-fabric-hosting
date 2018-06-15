using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public interface IConfigurableObjectDependenciesParameters
    {
        Action<IServiceCollection> DependenciesConfigAction { get; }
    }
}