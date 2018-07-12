using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseDelegateTheoryExtension
    {
        Delegate Delegate { get; }
    }
}