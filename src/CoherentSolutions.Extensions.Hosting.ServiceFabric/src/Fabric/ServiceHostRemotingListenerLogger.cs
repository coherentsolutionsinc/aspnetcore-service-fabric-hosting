namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRemotingListenerLogger : ServiceHostListenerLogger<ServiceListenerEventSourceData>
    {
        public ServiceHostRemotingListenerLogger(
            IServiceHostRemotingListenerInformation listenerInformation,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IServiceHostListenerLoggerOptions options)
            : base(listenerInformation, eventSource, eventCategoryName, options)
        {
        }
    }
}