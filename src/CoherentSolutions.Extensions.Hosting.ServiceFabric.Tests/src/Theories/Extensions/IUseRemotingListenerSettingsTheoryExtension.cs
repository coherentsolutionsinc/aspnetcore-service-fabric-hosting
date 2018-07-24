using System;

using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseRemotingListenerSettingsTheoryExtension
    {
        Func<FabricTransportRemotingListenerSettings> Factory { get; }
    }
}