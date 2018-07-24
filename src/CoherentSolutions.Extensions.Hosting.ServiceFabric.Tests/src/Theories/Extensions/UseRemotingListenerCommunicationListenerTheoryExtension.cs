using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingListenerCommunicationListenerTheoryExtension : IUseRemotingListenerCommunicationListenerTheoryExtension
    {
        public ServiceHostRemotingCommunicationListenerFactory Factory { get; private set; }

        public UseRemotingListenerCommunicationListenerTheoryExtension()
        {
            this.Factory = Tools.GetRemotingCommunicationListenerFunc();
        }

        public UseRemotingListenerCommunicationListenerTheoryExtension Setup(
            ServiceHostRemotingCommunicationListenerFactory factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}