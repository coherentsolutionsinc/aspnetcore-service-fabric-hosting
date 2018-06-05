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

        private static readonly AsyncLocal<ServiceAspNetCoreListenerRequestLoggerScope> listenerRequestScope;

        private readonly IServiceAspNetCoreListenerInformation listenerInformation;

        private readonly IServiceAspNetCoreListenerLoggerOptions loggerOptions;

        private readonly IServiceEventSource eventSource;

        private readonly string eventCategoryName;

        static ServiceAspNetCoreListenerLogger()
        {
            listenerRequestScope = new AsyncLocal<ServiceAspNetCoreListenerRequestLoggerScope>();
        }

        public ServiceAspNetCoreListenerLogger(
            IServiceAspNetCoreListenerInformation listenerInformation,
            IServiceAspNetCoreListenerLoggerOptions loggerOptions,
            IServiceEventSource eventSource,
            string categoryName)
        {
            this.listenerInformation = listenerInformation
             ?? throw new ArgumentNullException(nameof(listenerInformation));

            this.loggerOptions = loggerOptions
             ?? throw new ArgumentNullException(nameof(loggerOptions));

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

            if (this.loggerOptions.IncludeRequestInformation)
            {
                if (listenerRequestScope.Value != null)
                {
                    eventData.RequestId = listenerRequestScope.Value.RequestId;
                    eventData.RequestPath = listenerRequestScope.Value.RequestPath;
                }
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
            if (this.loggerOptions.LogLevel > logLevel)
            {
                return false;
            }
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
            if (listenerRequestScope.Value != null)
            {
                return null;
            }

            if (state is IEnumerable<KeyValuePair<string, object>> enumerable)
            {
                string requestId = null,
                       requestPath = null;

                foreach (var kv in enumerable)
                {
                    if (kv.Key.Equals(REQUEST_ID, StringComparison.OrdinalIgnoreCase))
                    {
                        requestId = kv.Value as string;
                    }

                    if (kv.Key.Equals(REQUEST_PATH, StringComparison.OrdinalIgnoreCase))
                    {
                        requestPath = kv.Value as string;
                    }
                }

                if (string.IsNullOrEmpty(requestId) || string.IsNullOrEmpty(requestPath))
                {
                    return null;
                }

                listenerRequestScope.Value = new ServiceAspNetCoreListenerRequestLoggerScope(
                    requestId,
                    requestPath,
                    () =>
                    {
                        listenerRequestScope.Value = null;
                    });

                return listenerRequestScope.Value;
            }

            return null;
        }
    }
}