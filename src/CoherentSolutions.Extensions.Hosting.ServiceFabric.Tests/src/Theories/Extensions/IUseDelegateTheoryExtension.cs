using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseDelegateTheoryExtension : ITheoryExtension
    {
        Delegate Delegate { get; }
    }
}