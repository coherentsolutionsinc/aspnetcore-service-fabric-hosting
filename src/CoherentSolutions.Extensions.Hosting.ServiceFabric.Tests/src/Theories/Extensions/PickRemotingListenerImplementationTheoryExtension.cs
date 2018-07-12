using System;

using Microsoft.ServiceFabric.Services.Remoting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickRemotingListenerImplementationTheoryExtension : IPickRemotingListenerImplementationTheoryExtension
    {
        public Action<IService> PickAction { get; private set; }

        public PickRemotingListenerImplementationTheoryExtension()
        {
            this.PickAction = s =>
            {
            };
        }

        public PickRemotingListenerImplementationTheoryExtension Setup(
            Action<IService> action)
        {
            this.PickAction = action ?? throw new ArgumentNullException(nameof(action));

            return this;
        }
    }
}