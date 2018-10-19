using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockServiceEventSource : IServiceEventSource
    {
        public void WriteEvent<T>(
            ref T eventData)
            where T : ServiceEventSourceData
        {
        }
    }
}