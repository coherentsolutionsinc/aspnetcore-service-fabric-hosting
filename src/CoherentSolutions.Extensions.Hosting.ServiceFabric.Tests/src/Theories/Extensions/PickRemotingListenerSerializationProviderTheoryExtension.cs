using System;

using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickRemotingListenerSerializationProviderTheoryExtension : IPickRemotingListenerSerializationProviderTheoryExtension
    {
        public Action<IServiceRemotingMessageSerializationProvider> PickAction { get; private set; }

        public PickRemotingListenerSerializationProviderTheoryExtension()
        {
            this.PickAction = s =>
            {
            };
        }

        public PickRemotingListenerSerializationProviderTheoryExtension Setup(
            Action<IServiceRemotingMessageSerializationProvider> action)
        {
            this.PickAction = action ?? throw new ArgumentNullException(nameof(action));

            return this;
        }
    }
}