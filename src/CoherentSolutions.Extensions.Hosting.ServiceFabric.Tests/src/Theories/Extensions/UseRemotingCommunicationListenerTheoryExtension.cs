using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingCommunicationListenerTheoryExtension : IUseRemotingCommunicationListenerTheoryExtension
    {
        public ServiceHostRemotingCommunicationListenerFactory Factory { get; private set; }

        public UseRemotingCommunicationListenerTheoryExtension()
        {
            this.Factory = Tools.GetRemotingCommunicationListenerFunc();
        }

        public UseRemotingCommunicationListenerTheoryExtension Setup(
            ServiceHostRemotingCommunicationListenerFactory factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}