using System.Diagnostics.Tracing;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    [EventData]
    public class ServiceListenerEventSourceData : ServiceEventSourceData
    {
        [EventField]
        public string EndpointName { get; set; }
    }
}