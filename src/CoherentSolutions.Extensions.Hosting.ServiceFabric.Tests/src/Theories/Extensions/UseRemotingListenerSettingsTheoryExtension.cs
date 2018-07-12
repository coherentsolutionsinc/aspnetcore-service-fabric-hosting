using System;

using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingListenerSettingsTheoryExtension : IUseRemotingListenerSettingsTheoryExtension
    {
        public Func<FabricTransportRemotingListenerSettings> Factory { get; private set; }

        public UseRemotingListenerSettingsTheoryExtension()
        {
            this.Factory = () => new FabricTransportRemotingListenerSettings();
        }

        public UseRemotingListenerSettingsTheoryExtension Setup(
            Func<FabricTransportRemotingListenerSettings> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}