using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceAspNetCoreListenerLogger : ILogger
    {
        private sealed class ServiceAspNetCoreListenerRequestLoggerScope : IDisposable
        {
            private bool disposed;

            public string RequestId { get; private set; }

            public string RequestPath { get; private set; }

            public Action OnDisposed { get; private set; }

            public ServiceAspNetCoreListenerRequestLoggerScope(
                string requestId,
                string requestPath,
                Action onDisposed)
            {
                this.RequestId = requestId
                 ?? throw new ArgumentNullException(nameof(requestId));

                this.RequestPath = requestPath
                 ?? throw new ArgumentNullException(nameof(requestPath));

                this.OnDisposed = onDisposed
                 ?? throw new ArgumentNullException(nameof(onDisposed));
            }

            public void Dispose()
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;

                Thread.MemoryBarrier();

                this.OnDisposed();

                this.RequestId = null;
                this.RequestPath = null;
                this.OnDisposed = null;
            }
        }

        [EventData]
        private class ServiceAspNetCoreListenerLoggerEventSourceData : ServiceEventSourceData
        {
            public string RequestId { get; set; }

            public string RequestPath { get; set; }

            public string EndpointName { get; set; }
        }

        private const string REQUEST_ID = "RequestId";

        private const string REQUEST_PATH = "RequestPath";

        private readonly IServiceAspNetCoreListenerInformation listenerInformation;

        private readonly AsyncLocal<ServiceAspNetCoreListenerRequestLoggerScope> listenerRequestScope;

        private readonly IServiceEventSource eventSource;

        private readonly string eventCategoryName;

        public ServiceAspNetCoreListenerLogger(
            IServiceAspNetCoreListenerInformation listenerInformation,
            IServiceEventSource eventSource,
            string categoryName)
        {
            this.listenerInformation = listenerInformation
             ?? throw new ArgumentNullException(nameof(listenerInformation));

            this.listenerRequestScope = new AsyncLocal<ServiceAspNetCoreListenerRequestLoggerScope>();

            this.eventSource = eventSource
             ?? throw new ArgumentNullException(nameof(eventSource));

            this.eventCategoryName = categoryName
             ?? throw new ArgumentNullException(nameof(categoryName));
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var eventMessage = formatter(state, exception);
            var eventData = new ServiceAspNetCoreListenerLoggerEventSourceData
            {
                EventCategoryName = this.eventCategoryName,
                EndpointName = this.listenerInformation.EndpointName
            };

            if (this.listenerRequestScope.Value != null)
            {
                eventData.RequestId = this.listenerRequestScope.Value.RequestId;
                eventData.RequestPath = this.listenerRequestScope.Value.RequestPath;
            }

            switch (logLevel)
            {
                case LogLevel.None:
                case LogLevel.Trace:
                case LogLevel.Debug:
                    this.eventSource.Verbose(eventId.Id, eventId.Name, this.eventCategoryName, eventMessage, eventData);
                    break;
                case LogLevel.Information:
                    this.eventSource.Information(eventId.Id, eventId.Name, this.eventCategoryName, eventMessage, eventData);
                    break;
                case LogLevel.Warning:
                    this.eventSource.Warning(eventId.Id, eventId.Name, this.eventCategoryName, eventMessage, eventData);
                    break;
                case LogLevel.Error:
                    this.eventSource.Error(eventId.Id, eventId.Name, this.eventCategoryName, eventMessage, eventData);
                    break;
                case LogLevel.Critical:
                    this.eventSource.Critical(eventId.Id, eventId.Name, this.eventCategoryName, eventMessage, eventData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public bool IsEnabled(
            LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.None:
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return this.eventSource.IsEnabled(EventLevel.Verbose);
                case LogLevel.Information:
                    return this.eventSource.IsEnabled(EventLevel.Informational);
                case LogLevel.Warning:
                    return this.eventSource.IsEnabled(EventLevel.Warning);
                case LogLevel.Error:
                    return this.eventSource.IsEnabled(EventLevel.Error);
                case LogLevel.Critical:
                    return this.eventSource.IsEnabled(EventLevel.Critical);
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public IDisposable BeginScope<TState>(
            TState state)
        {
            if (this.listenerRequestScope.Value == null && state is IDictionary<string, object> dictionary)
            {
                if (dictionary.TryGetValue(REQUEST_ID, out var requestId) && dictionary.TryGetValue(REQUEST_PATH, out var requestPath))
                {
                    this.listenerRequestScope.Value = new ServiceAspNetCoreListenerRequestLoggerScope(
                        requestId as string,
                        requestPath as string,
                        () =>
                        {
                            this.listenerRequestScope.Value = null;
                        });

                    return this.listenerRequestScope.Value;
                }
            }

            return null;
        }
    }
}