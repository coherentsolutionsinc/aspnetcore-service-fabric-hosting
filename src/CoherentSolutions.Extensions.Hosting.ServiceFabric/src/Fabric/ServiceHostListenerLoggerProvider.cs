using System;
using System.Collections.Concurrent;
using System.Threading;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostListenerLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> loggers;

        private bool disposed;

        protected ServiceHostListenerLoggerProvider()
        {
            this.loggers = new ConcurrentDictionary<string, ILogger>(StringComparer.OrdinalIgnoreCase);
        }

        protected abstract ILogger CreateLoggerInstance(
            string categoryName);

        public ILogger CreateLogger(
            string categoryName)
        {
            categoryName = categoryName ?? string.Empty;

            return this.loggers.GetOrAdd(categoryName, key => this.CreateLoggerInstance(categoryName));
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