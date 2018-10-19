using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRemotingListenerLoggerProvider : ServiceHostLoggerProvider
    {
        private readonly IServiceHostRemotingListenerInformation listenerInformation;

        private readonly ServiceContext serviceContext;

        private readonly IConfigurableObjectLoggerOptions loggerOptions;

        private readonly IServiceEventSource eventSource;

        public ServiceHostRemotingListenerLoggerProvider(
            IServiceHostRemotingListenerInformation listenerInformation,
            ServiceContext serviceContext,
            IServiceEventSource eventSource,
            IConfigurableObjectLoggerOptions loggerOptions)
        {
            this.serviceContext = serviceContext
             ?? throw new ArgumentNullException(nameof(serviceContext));

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
            return new ServiceHostRemotingListenerLogger(
                this.listenerInformation,
                this.serviceContext,
                this.eventSource,
                categoryName,
                this.loggerOptions);
        }
    }
}