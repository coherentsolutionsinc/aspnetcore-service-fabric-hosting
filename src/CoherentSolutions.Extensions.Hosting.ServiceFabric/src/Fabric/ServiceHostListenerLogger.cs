using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostListenerLogger<TEventSourceData> : ServiceHostLogger<TEventSourceData>
        where TEventSourceData : ServiceListenerEventSourceData, new()
    {
        private readonly IServiceHostListenerInformation listenerInformation;

        protected ServiceHostListenerLogger(
            IServiceHostListenerInformation listenerInformation,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IServiceHostListenerLoggerOptions options)
            : base(eventSource, eventCategoryName, options)
        {
            this.listenerInformation = listenerInformation
             ?? throw new ArgumentNullException(nameof(listenerInformation));
        }

        protected override void FillEventData<TState>(
            TState state,
            TEventSourceData eventData)
        {
            eventData.EndpointName = this.listenerInformation.EndpointName;
        }
    }
}