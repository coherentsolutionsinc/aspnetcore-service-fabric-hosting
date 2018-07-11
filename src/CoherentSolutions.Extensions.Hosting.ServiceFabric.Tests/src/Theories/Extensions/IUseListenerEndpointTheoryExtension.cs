namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseListenerEndpointTheoryExtension : ITheoryExtension
    {
        string Endpoint { get; }
    }
}