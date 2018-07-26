using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IConfigureDependenciesTheoryExtension
    {
        Action<IServiceCollection> ConfigAction { get; }
    }
}