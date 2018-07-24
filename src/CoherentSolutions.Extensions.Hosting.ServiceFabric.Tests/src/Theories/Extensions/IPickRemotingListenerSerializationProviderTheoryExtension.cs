using System;

using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IPickRemotingListenerSerializationProviderTheoryExtension
    {
        Action<IServiceRemotingMessageSerializationProvider> PickAction { get; }
    }
}