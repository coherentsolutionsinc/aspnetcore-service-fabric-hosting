namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegateLogger : ServiceHostLogger<ServiceEventSourceData>
    {
        public ServiceHostDelegateLogger(
            IServiceEventSource eventSource,
            string eventCategoryName,
            IServiceHostLoggerOptions options)
            : base(eventSource, eventCategoryName, options)
        {
        }

        protected override void FillEventData<TState>(
            TState state,
            ServiceEventSourceData eventData)
        {
        }
    }
}