using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseAspNetCoreCommunicationListenerTheoryExtension : ITheoryExtension
    {
        ServiceHostAspNetCoreCommunicationListenerFactory Factory { get; }
    }
}