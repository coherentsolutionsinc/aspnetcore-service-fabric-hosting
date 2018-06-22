namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRemotingListenerLogger : ServiceHostListenerLogger<ServiceListenerEventSourceData>
    {
        public ServiceHostRemotingListenerLogger(
            IServiceHostRemotingListenerInformation listenerInformation,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IServiceHostLoggerOptions options)
            : base(listenerInformation, eventSource, eventCategoryName, options)
        {
        }
    }
}