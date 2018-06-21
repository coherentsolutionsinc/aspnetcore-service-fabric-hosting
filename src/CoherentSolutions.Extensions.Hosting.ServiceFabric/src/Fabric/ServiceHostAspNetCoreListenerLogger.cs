namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostAspNetCoreListenerLogger : ServiceHostListenerLogger<ServiceListenerEventSourceData>
    {
        public ServiceHostAspNetCoreListenerLogger(
            IServiceHostAspNetCoreListenerInformation listenerInformation,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IServiceHostListenerLoggerOptions options)
            : base(listenerInformation, eventSource, eventCategoryName, options)
        {
        }
    }
}