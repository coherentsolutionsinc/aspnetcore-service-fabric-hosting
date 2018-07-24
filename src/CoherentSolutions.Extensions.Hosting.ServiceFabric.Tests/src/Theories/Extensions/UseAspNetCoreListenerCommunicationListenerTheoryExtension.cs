using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseAspNetCoreListenerCommunicationListenerTheoryExtension : IUseAspNetCoreListenerCommunicationListenerTheoryExtension
    {
        public ServiceHostAspNetCoreCommunicationListenerFactory Factory { get; private set; }

        public UseAspNetCoreListenerCommunicationListenerTheoryExtension()
        {
            this.Factory = Tools.GetAspNetCoreCommunicationListenerFunc();
        }

        public UseAspNetCoreListenerCommunicationListenerTheoryExtension Setup(
            ServiceHostAspNetCoreCommunicationListenerFactory factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}