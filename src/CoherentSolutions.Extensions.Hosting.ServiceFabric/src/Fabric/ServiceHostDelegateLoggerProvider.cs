using System;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegateLoggerProvider : ServiceHostLoggerProvider
    {
        private readonly IServiceHostLoggerOptions loggerOptions;

        private readonly IServiceEventSource eventSource;

        public ServiceHostDelegateLoggerProvider(
            IServiceHostLoggerOptions loggerOptions,
            IServiceEventSource eventSource)
        {
            this.loggerOptions = loggerOptions
             ?? throw new ArgumentNullException(nameof(loggerOptions));

            this.eventSource = eventSource
             ?? throw new ArgumentNullException(nameof(eventSource));
        }

        protected override ILogger CreateLoggerInstance(
            string categoryName)
        {
            return new ServiceHostDelegateLogger(this.eventSource, categoryName, this.loggerOptions);
        }
    }
}