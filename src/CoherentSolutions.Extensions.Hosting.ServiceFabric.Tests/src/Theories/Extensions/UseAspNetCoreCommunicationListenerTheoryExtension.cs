using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseAspNetCoreCommunicationListenerTheoryExtension : IUseAspNetCoreCommunicationListenerTheoryExtension
    {
        public ServiceHostAspNetCoreCommunicationListenerFactory Factory { get; private set; }

        public UseAspNetCoreCommunicationListenerTheoryExtension()
        {
            this.Factory = Tools.GetAspNetCoreCommunicationListenerFunc();
        }

        public UseAspNetCoreCommunicationListenerTheoryExtension Setup(
            ServiceHostAspNetCoreCommunicationListenerFactory factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}