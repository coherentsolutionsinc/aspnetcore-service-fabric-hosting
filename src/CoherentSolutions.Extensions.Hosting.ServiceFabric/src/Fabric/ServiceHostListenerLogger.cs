using System;
using System.Diagnostics.Tracing;
using System.Threading;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostListenerLogger<TEventSourceData> : ILogger
        where TEventSourceData : ServiceListenerEventSourceData, new()
    {
        private sealed class Scope : IDisposable
        {
            private readonly AsyncLocal<Scope> current;

            private readonly Scope parent;

            private bool disposed;

            public string Metadata { get; }

            public Scope(
                AsyncLocal<Scope> current,
                object state)
            {
                this.current = current;
                this.parent = current.Value;

                this.Metadata = this.parent != null
                    ? string.Join(Environment.NewLine, this.parent?.Metadata, state)
                    : state.ToString();
            }

            public void Dispose()
            {
                if (this.disposed)
                {
                    return;
                }

                this.current.Value = this.parent;
                this.disposed = true;
            }
        }

        private readonly IServiceHostListenerInformation listenerInformation;

        private readonly IServiceEventSource eventSource;

        private readonly string eventCategoryName;

        private readonly AsyncLocal<Scope> scope;

        private readonly IServiceHostListenerLoggerOptions options;

        protected ServiceHostListenerLogger(
            IServiceHostListenerInformation listenerInformation,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IServiceHostListenerLoggerOptions options)
        {
            this.listenerInformation = listenerInformation
             ?? throw new ArgumentNullException(nameof(listenerInformation));

            this.eventSource = eventSource
             ?? throw new ArgumentNullException(nameof(eventSource));

            this.eventCategoryName = eventCategoryName
             ?? throw new ArgumentNullException(nameof(eventCategoryName));

            this.scope = new AsyncLocal<Scope>();

            this.options = options
             ?? throw new ArgumentNullException(nameof(options));
        }

        protected abstract void FillEventData<TState>(
            TState state,
            TEventSourceData eventData);

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var eventMessage = formatter(state, exception);
            var eventData = new TEventSourceData
            {
                EventCategoryName = this.eventCategoryName,
                EventMetadata = this.scope.Value?.Metadata,
                EndpointName = this.listenerInformation.EndpointName
            };

            if (this.options.IncludeMetadata && this.scope.Value != null)
            {
                eventData.EventMetadata = this.scope.Value.Metadata;
            }

            if (this.options.IncludeExceptionStackTrace && exception != null)
            {
                eventData.EventStackTrace = exception.StackTrace;
            }

            this.FillEventData(state, eventData);

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
            if (this.options.LogLevel > logLevel)
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
            return this.scope.Value = new Scope(this.scope, state);
        }
    }
}