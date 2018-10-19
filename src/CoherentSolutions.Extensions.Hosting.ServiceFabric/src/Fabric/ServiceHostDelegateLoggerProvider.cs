using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegateLoggerProvider : ServiceHostLoggerProvider
    {
        private readonly ServiceContext serviceContext;

        private readonly IConfigurableObjectLoggerOptions loggerOptions;

        private readonly IServiceEventSource eventSource;

        public ServiceHostDelegateLoggerProvider(
            ServiceContext serviceContext,
            IServiceEventSource eventSource,
            IConfigurableObjectLoggerOptions loggerOptions)
        {
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
            return new ServiceHostDelegateLogger(
                this.serviceContext,
                this.eventSource,
                categoryName,
                this.loggerOptions);
        }
    }
}