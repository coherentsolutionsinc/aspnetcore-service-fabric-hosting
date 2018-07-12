using System;

using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickRemotingListenerSettingsTheoryExtension : IPickRemotingListenerSettingsTheoryExtension
    {
        public Action<FabricTransportRemotingListenerSettings> PickAction { get; private set; }

        public PickRemotingListenerSettingsTheoryExtension()
        {
            this.PickAction = s =>
            {
            };
        }

        public PickRemotingListenerSettingsTheoryExtension Setup(
            Action<FabricTransportRemotingListenerSettings> action)
        {
            this.PickAction = action ?? throw new ArgumentNullException(nameof(action));

            return this;
        }
    }
}