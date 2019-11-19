using System;
using System.Collections.Generic;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IPickDependencyTheoryExtension : ITheoryExtension
    {
        IEnumerable<Action<IServiceProvider>> PickActions { get; }
    }
}