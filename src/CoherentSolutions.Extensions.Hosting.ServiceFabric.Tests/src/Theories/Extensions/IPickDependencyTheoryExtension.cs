using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IPickDependencyTheoryExtension : ITheoryExtension
    {
        IEnumerable<Action<IServiceProvider>> PickActions { get; }
    }
}