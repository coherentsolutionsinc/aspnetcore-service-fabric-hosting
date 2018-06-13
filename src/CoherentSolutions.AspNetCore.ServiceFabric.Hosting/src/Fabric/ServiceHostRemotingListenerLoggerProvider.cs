using System;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceHostRemotingListenerLoggerProvider : ServiceHostListenerLoggerProvider
    {
        private readonly IServiceHostRemotingListenerInformation listenerInformation;

        private readonly IServiceHostListenerLoggerOptions loggerOptions;

        private readonly IServiceEventSource eventSource;

        public ServiceHostRemotingListenerLoggerProvider(
            IServiceHostRemotingListenerInformation listenerInformation,
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
            return new ServiceHostRemotingListenerLogger(this.listenerInformation, this.eventSource, categoryName, this.loggerOptions);
        }
    }
}