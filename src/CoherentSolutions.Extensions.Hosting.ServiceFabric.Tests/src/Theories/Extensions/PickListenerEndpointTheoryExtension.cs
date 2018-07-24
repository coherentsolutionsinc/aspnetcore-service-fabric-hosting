using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class PickListenerEndpointTheoryExtension : IPickListenerEndpointTheoryExtension
    {
        public Action<string> PickAction { get; private set; }

        public PickListenerEndpointTheoryExtension()
        {
            this.PickAction = s =>
            {
            };
        }

        public PickListenerEndpointTheoryExtension Setup(
            Action<string> action)
        {
            this.PickAction = action ?? throw new ArgumentNullException(nameof(action));

            return this;
        }
    }
}