using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostGenericListenerLogger : ServiceHostListenerLogger<ServiceListenerEventSourceData>
    {
        public ServiceHostGenericListenerLogger(
            IServiceHostGenericListenerInformation listenerInformation,
            ServiceContext serviceContext,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IConfigurableObjectLoggerOptions options)
            : base(listenerInformation, serviceContext, eventSource, eventCategoryName, options)
        {
        }
    }
}