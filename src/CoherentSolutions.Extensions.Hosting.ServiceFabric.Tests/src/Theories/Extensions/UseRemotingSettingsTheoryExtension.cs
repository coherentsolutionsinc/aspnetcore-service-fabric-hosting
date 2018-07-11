using System;

using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingSettingsTheoryExtension : IUseRemotingSettingsTheoryExtension
    {
        public Func<FabricTransportRemotingListenerSettings> Factory { get; private set; }

        public UseRemotingSettingsTheoryExtension()
        {
            this.Factory = () => new FabricTransportRemotingListenerSettings();
        }

        public UseRemotingSettingsTheoryExtension Setup(
            Func<FabricTransportRemotingListenerSettings> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}