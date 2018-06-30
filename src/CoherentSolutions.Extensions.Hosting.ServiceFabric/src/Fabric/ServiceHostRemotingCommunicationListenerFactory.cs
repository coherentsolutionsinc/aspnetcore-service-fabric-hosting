using System.Fabric;

using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public delegate FabricTransportServiceRemotingListener ServiceHostRemotingCommunicationListenerFactory(
        ServiceContext context,
        ServiceHostRemotingCommunicationListenerComponentsFactory build);
}