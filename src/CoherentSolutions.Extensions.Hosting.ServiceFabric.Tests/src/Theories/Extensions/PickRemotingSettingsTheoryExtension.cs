using System;

using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickRemotingSettingsTheoryExtension : IPickRemotingSettingsTheoryExtension
    {
        public Action<FabricTransportRemotingListenerSettings> PickAction { get; private set; }

        public PickRemotingSettingsTheoryExtension()
        {
            this.PickAction = s =>
            {
            };
        }

        public PickRemotingSettingsTheoryExtension Setup(
            Action<FabricTransportRemotingListenerSettings> action)
        {
            this.PickAction = action ?? throw new ArgumentNullException(nameof(action));

            return this;
        }
    }
}