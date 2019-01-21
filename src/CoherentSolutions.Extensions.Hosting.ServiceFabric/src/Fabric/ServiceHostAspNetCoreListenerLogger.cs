using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostAspNetCoreListenerLogger : ServiceHostListenerLogger<ServiceListenerEventSourceData>
    {
        public ServiceHostAspNetCoreListenerLogger(
            IServiceHostAspNetCoreListenerInformation listenerInformation,
            ServiceContext serviceContext,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IConfigurableObjectLoggerOptions options)
            : base(listenerInformation, serviceContext, eventSource, eventCategoryName, options)
        {
        }
    }
}