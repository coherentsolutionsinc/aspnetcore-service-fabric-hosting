using System;

using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickRemotingSerializerTheoryExtension : IPickRemotingSerializerTheoryExtension
    {
        public Action<IServiceRemotingMessageSerializationProvider> PickAction { get; private set; }

        public PickRemotingSerializerTheoryExtension()
        {
            this.PickAction = s =>
            {
            };
        }

        public PickRemotingSerializerTheoryExtension Setup(
            Action<IServiceRemotingMessageSerializationProvider> action)
        {
            this.PickAction = action ?? throw new ArgumentNullException(nameof(action));

            return this;
        }
    }
}