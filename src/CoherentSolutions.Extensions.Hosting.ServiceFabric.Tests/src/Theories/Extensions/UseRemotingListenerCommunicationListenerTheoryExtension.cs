using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingListenerCommunicationListenerTheoryExtension : IUseRemotingListenerCommunicationListenerTheoryExtension
    {
        public ServiceHostRemotingCommunicationListenerFactory Factory { get; private set; }

        public UseRemotingListenerCommunicationListenerTheoryExtension()
        {
            this.Factory = (
                context,
                build) =>
            {
                var options = build(context);
                return new MockFabricTransportServiceRemotingListener(
                    context,
                    options.MessageHandler,
                    options.ListenerSettings,
                    options.MessageSerializationProvider);
            };
        }

        public UseRemotingListenerCommunicationListenerTheoryExtension Setup(
            ServiceHostRemotingCommunicationListenerFactory factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}