using System;

using Microsoft.ServiceFabric.Services.Remoting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IPickRemotingListenerImplementationTheoryExtension
    {
        Action<IService> PickAction { get; }
    }
}