using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseDependenciesTheoryExtension
    {
        Func<IServiceCollection> Factory { get; }
    }
}