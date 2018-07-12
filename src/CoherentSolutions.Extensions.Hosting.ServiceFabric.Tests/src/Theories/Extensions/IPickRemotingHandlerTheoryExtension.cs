using System;

using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IPickRemotingHandlerTheoryExtension : ITheoryExtension
    {
        Action<IServiceRemotingMessageHandler> PickAction { get; }
    }
}