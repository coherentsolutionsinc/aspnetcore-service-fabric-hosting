using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseGenericListenerCommunicationListenerTheoryExtension : IUseGenericListenerCommunicationListenerTheoryExtension
    {
        public ServiceHostGenericCommunicationListenerFactory Factory { get; private set; }

        public UseGenericListenerCommunicationListenerTheoryExtension()
        {
            this.Factory = (
                context,
                endpointName,
                provider) =>
            {
                return new MockGenericCommunicationListener();
            };
        }

        public UseGenericListenerCommunicationListenerTheoryExtension Setup(
            ServiceHostGenericCommunicationListenerFactory factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}