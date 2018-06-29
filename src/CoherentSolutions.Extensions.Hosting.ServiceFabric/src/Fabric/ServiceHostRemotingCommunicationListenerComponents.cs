using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRemotingCommunicationListenerComponents
    {
        public ServiceRemotingMessageDispatcher MessageDispatcher { get; }

        public IServiceRemotingMessageSerializationProvider MessageSerializationProvider { get; }

        public FabricTransportRemotingListenerSettings ListenerSettings { get; }

        public ServiceHostRemotingCommunicationListenerComponents(
            ServiceRemotingMessageDispatcher messageDispatcher,
            IServiceRemotingMessageSerializationProvider messageSerializationProvider,
            FabricTransportRemotingListenerSettings listenerSettings)
        {
            this.MessageDispatcher = messageDispatcher;
            this.ListenerSettings = listenerSettings;
            this.MessageSerializationProvider = messageSerializationProvider;
        }
    }
}