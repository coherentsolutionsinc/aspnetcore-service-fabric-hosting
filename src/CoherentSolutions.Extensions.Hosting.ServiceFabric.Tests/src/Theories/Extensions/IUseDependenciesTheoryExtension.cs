using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseDependenciesTheoryExtension : ITheoryExtension
    {
        Func<IServiceCollection> Factory { get; }
    }
}