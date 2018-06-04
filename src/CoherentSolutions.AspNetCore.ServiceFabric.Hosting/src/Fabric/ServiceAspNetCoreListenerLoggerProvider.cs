using System;
using System.Collections.Concurrent;
using System.Threading;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceAspNetCoreListenerLoggerProvider : ILoggerProvider
    {
        private readonly IServiceAspNetCoreListenerInformation listenerInformation;

        private readonly IServiceEventSource eventSource;

        private readonly ConcurrentDictionary<string, ServiceAspNetCoreListenerLogger> loggers;

        private bool disposed;

        public ServiceAspNetCoreListenerLoggerProvider(
            IServiceAspNetCoreListenerInformation listenerInformation,
            IServiceEventSource eventSource)
        {
            this.listenerInformation = listenerInformation
             ?? throw new ArgumentNullException(nameof(listenerInformation));

            this.eventSource = eventSource
             ?? throw new ArgumentNullException(nameof(eventSource));

            this.loggers = new ConcurrentDictionary<string, ServiceAspNetCoreListenerLogger>(StringComparer.OrdinalIgnoreCase);
        }

        public ILogger CreateLogger(
            string categoryName)
        {
            categoryName = categoryName ?? string.Empty;

            return this.loggers.GetOrAdd(
                categoryName,
                key => new ServiceAspNetCoreListenerLogger(this.listenerInformation, this.eventSource, categoryName));
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(
            bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.disposed = true;

                Thread.MemoryBarrier();

                this.loggers.Clear();
            }
        }
    }
}