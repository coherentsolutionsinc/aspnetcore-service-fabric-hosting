using System;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostAspNetCoreListenerLoggerProvider : ServiceHostListenerLoggerProvider
    {
        private readonly IServiceHostAspNetCoreListenerInformation listenerInformation;

        private readonly IServiceHostListenerLoggerOptions loggerOptions;

        private readonly IServiceEventSource eventSource;

        public ServiceHostAspNetCoreListenerLoggerProvider(
            IServiceHostAspNetCoreListenerInformation listenerInformation,
            IServiceHostListenerLoggerOptions loggerOptions,
            IServiceEventSource eventSource)
        {
            this.listenerInformation = listenerInformation
             ?? throw new ArgumentNullException(nameof(listenerInformation));

            this.loggerOptions = loggerOptions
             ?? throw new ArgumentNullException(nameof(loggerOptions));

            this.eventSource = eventSource
             ?? throw new ArgumentNullException(nameof(eventSource));
        }

        protected override ILogger CreateLoggerInstance(
            string categoryName)
        {
            return new ServiceHostAspNetCoreListenerLogger(this.listenerInformation, this.eventSource, categoryName, this.loggerOptions);
        }
    }
}