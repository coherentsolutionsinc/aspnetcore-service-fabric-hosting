using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools
{
    public interface IConfigurableObjectDependenciesParameters
    {
        Action<IServiceCollection> DependenciesConfigAction { get; }
    }
}