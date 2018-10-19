using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostListenerLogger<TEventSourceData> : ServiceHostLogger<TEventSourceData>
        where TEventSourceData : ServiceListenerEventSourceData, new()
    {
        private readonly IServiceHostListenerInformation listenerInformation;

        protected ServiceHostListenerLogger(
            IServiceHostListenerInformation listenerInformation,
            ServiceContext serviceContext,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IConfigurableObjectLoggerOptions options)
            : base(serviceContext, eventSource, eventCategoryName, options)
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