using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseAspNetCoreListenerCommunicationListenerTheoryExtension
    {
        ServiceHostAspNetCoreCommunicationListenerFactory Factory { get; }
    }
}