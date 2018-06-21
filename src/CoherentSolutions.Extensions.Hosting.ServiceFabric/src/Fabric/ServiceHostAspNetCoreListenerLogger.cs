namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostAspNetCoreListenerLogger : ServiceHostListenerLogger<ServiceListenerEventSourceData>
    {
        public ServiceHostAspNetCoreListenerLogger(
            IServiceHostAspNetCoreListenerInformation listenerInformation,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IServiceHostLoggerOptions options)
            : base(listenerInformation, eventSource, eventCategoryName, options)
        {
        }
    }
}