using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegateLogger : ServiceHostLogger<ServiceEventSourceData>
    {
        public ServiceHostDelegateLogger(
            ServiceContext serviceContext,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IConfigurableObjectLoggerOptions options)
            : base(serviceContext, eventSource, eventCategoryName, options)
        {
        }

        protected override void FillEventData<TState>(
            TState state,
            ServiceEventSourceData eventData)
        {
        }
    }
}