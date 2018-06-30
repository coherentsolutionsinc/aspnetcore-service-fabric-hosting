using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public delegate ServiceHostRemotingCommunicationListenerComponents ServiceHostRemotingCommunicationListenerComponentsFactory(
        ServiceContext context);
}