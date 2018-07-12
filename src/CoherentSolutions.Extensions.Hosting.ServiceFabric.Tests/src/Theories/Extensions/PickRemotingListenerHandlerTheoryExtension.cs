using System;

using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickRemotingListenerHandlerTheoryExtension : IPickRemotingListenerHandlerTheoryExtension
    {
        public Action<IServiceRemotingMessageHandler> PickAction { get; private set; }

        public PickRemotingListenerHandlerTheoryExtension()
        {
            this.PickAction = s =>
            {
            };
        }

        public PickRemotingListenerHandlerTheoryExtension Setup(
            Action<IServiceRemotingMessageHandler> action)
        {
            this.PickAction = action ?? throw new ArgumentNullException(nameof(action));

            return this;
        }
    }
}