using System;

using Microsoft.ServiceFabric.Services.Remoting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IPickRemotingImplementationTheoryExtension : ITheoryExtension
    {
        Action<IService> PickAction { get; }
    }
}