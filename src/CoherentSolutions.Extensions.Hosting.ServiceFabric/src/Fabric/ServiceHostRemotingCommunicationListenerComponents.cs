using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRemotingCommunicationListenerComponents
    {
        public IServiceRemotingMessageHandler MessageHandler { get; }

        public IServiceRemotingMessageSerializationProvider MessageSerializationProvider { get; }

        public FabricTransportRemotingListenerSettings ListenerSettings { get; }

        public ServiceHostRemotingCommunicationListenerComponents(
            IServiceRemotingMessageHandler messageDispatcher,
            IServiceRemotingMessageSerializationProvider messageSerializationProvider,
            FabricTransportRemotingListenerSettings listenerSettings)
        {
            this.MessageHandler = messageDispatcher;
            this.ListenerSettings = listenerSettings;
            this.MessageSerializationProvider = messageSerializationProvider;
        }
    }
}