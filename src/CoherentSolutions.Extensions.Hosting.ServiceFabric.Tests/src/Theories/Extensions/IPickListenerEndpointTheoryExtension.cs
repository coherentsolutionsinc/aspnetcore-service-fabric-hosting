using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IPickListenerEndpointTheoryExtension : ITheoryExtension
    {
        Action<string> PickAction { get; }
    }
}