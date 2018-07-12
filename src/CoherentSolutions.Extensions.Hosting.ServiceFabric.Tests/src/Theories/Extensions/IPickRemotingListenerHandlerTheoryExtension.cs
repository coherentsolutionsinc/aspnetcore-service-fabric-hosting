using System;

using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IPickRemotingListenerHandlerTheoryExtension
    {
        Action<IServiceRemotingMessageHandler> PickAction { get; }
    }
}