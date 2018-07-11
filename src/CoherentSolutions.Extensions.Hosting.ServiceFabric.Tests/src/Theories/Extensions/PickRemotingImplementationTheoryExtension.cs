using System;

using Microsoft.ServiceFabric.Services.Remoting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickRemotingImplementationTheoryExtension : IPickRemotingImplementationTheoryExtension
    {
        public Action<IService> PickAction { get; private set; }

        public PickRemotingImplementationTheoryExtension()
        {
            this.PickAction = s =>
            {
            };
        }

        public PickRemotingImplementationTheoryExtension Setup(
            Action<IService> action)
        {
            this.PickAction = action ?? throw new ArgumentNullException(nameof(action));

            return this;
        }
    }
}