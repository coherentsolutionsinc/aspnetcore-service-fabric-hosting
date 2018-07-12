using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IPickListenerEndpointTheoryExtension
    {
        Action<string> PickAction { get; }
    }
}