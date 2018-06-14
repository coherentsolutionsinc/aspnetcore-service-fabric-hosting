using System.Diagnostics.Tracing;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    [EventData]
    public class ServiceListenerEventSourceData : ServiceEventSourceData
    {
        [EventField]
        public string EndpointName { get; set; }
    }
}