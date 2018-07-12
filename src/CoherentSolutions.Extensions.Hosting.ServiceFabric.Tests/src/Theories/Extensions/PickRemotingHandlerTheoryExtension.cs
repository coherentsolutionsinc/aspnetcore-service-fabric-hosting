using System;

using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickRemotingHandlerTheoryExtension : IPickRemotingHandlerTheoryExtension
    {
        public Action<IServiceRemotingMessageHandler> PickAction { get; private set; }

        public PickRemotingHandlerTheoryExtension()
        {
            this.PickAction = s =>
            {
            };
        }

        public PickRemotingHandlerTheoryExtension Setup(
            Action<IServiceRemotingMessageHandler> action)
        {
            this.PickAction = action ?? throw new ArgumentNullException(nameof(action));

            return this;
        }
    }
}