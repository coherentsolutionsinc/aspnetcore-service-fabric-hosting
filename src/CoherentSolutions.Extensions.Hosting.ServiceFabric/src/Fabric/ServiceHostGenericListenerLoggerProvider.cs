using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostGenericListenerLoggerProvider : ServiceHostLoggerProvider
    {
        private readonly IServiceHostGenericListenerInformation listenerInformation;

        private readonly ServiceContext serviceContext;

        private readonly IServiceEventSource eventSource;

        private readonly IConfigurableObjectLoggerOptions loggerOptions;

        public ServiceHostGenericListenerLoggerProvider(
            IServiceHostGenericListenerInformation listenerInformation,
            ServiceContext serviceContext,
            IServiceEventSource eventSource,
            IConfigurableObjectLoggerOptions loggerOptions)
        {
            this.listenerInformation = listenerInformation
             ?? throw new ArgumentNullException(nameof(listenerInformation));

            this.serviceContext = serviceContext
             ?? throw new ArgumentNullException(nameof(serviceContext));

            this.loggerOptions = loggerOptions
             ?? throw new ArgumentNullException(nameof(loggerOptions));

            this.eventSource = eventSource
             ?? throw new ArgumentNullException(nameof(eventSource));
        }

        protected override ILogger CreateLoggerInstance(
            string categoryName)
        {
            return new ServiceHostGenericListenerLogger(
                this.listenerInformation,
                this.serviceContext,
                this.eventSource,
                categoryName,
                this.loggerOptions);
        }
    }
}