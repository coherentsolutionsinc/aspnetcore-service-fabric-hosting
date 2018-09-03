using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseAspNetCoreListenerCommunicationListenerTheoryExtension : IUseAspNetCoreListenerCommunicationListenerTheoryExtension
    {
        public ServiceHostAspNetCoreCommunicationListenerFactory Factory { get; private set; }

        public UseAspNetCoreListenerCommunicationListenerTheoryExtension()
        {
            this.Factory = (
                context,
                name,
                factory) => new MockAspNetCoreCommunicationListener(context, factory);
        }

        public UseAspNetCoreListenerCommunicationListenerTheoryExtension Setup(
            ServiceHostAspNetCoreCommunicationListenerFactory factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}