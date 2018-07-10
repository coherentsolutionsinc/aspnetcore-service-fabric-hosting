using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IResolveDependencyTheoryExtension : ITheoryExtension
    {
        IEnumerable<Action<IServiceProvider>> ServiceResolveDelegates { get; }
    }
}