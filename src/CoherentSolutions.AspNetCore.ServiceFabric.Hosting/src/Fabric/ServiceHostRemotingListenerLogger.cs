namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
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

        protected override void FillEventData<TState>(
            TState state,
            ServiceListenerEventSourceData eventData)
        {
        }
    }
}