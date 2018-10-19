using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Threading;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostLogger<TEventSourceData> : ILogger
        where TEventSourceData : ServiceEventSourceData, new()
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

        private readonly ServiceContext serviceContext;

        private readonly IServiceEventSource eventSource;

        private readonly string eventCategoryName;

        private readonly AsyncLocal<Scope> scope;

        private readonly IConfigurableObjectLoggerOptions options;

        protected ServiceHostLogger(
            ServiceContext serviceContext,
            IServiceEventSource eventSource,
            string eventCategoryName,
            IConfigurableObjectLoggerOptions options)
        {
            this.serviceContext = serviceContext
             ?? throw new ArgumentNullException(nameof(serviceContext));

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
            var eventData = new TEventSourceData();

            this.FillEventData(state, eventData);

            eventData.EventId = eventId.Id;
            eventData.EventName = eventId.Name;
            eventData.EventLevel = GetEventLevel(logLevel);
            eventData.EventMessage = formatter(state, exception);
            eventData.EventCategoryName = this.eventCategoryName;
            eventData.ServiceName = this.serviceContext.ServiceName.AbsoluteUri;
            eventData.ServiceTypeName = this.serviceContext.ServiceTypeName;
            eventData.ReplicaOrInstanceId = this.serviceContext.ReplicaOrInstanceId;
            eventData.PartitionId = this.serviceContext.PartitionId;
            eventData.ApplicationName = this.serviceContext.CodePackageActivationContext.ApplicationName;
            eventData.ApplicationTypeName = this.serviceContext.CodePackageActivationContext.ApplicationTypeName;
            eventData.NodeName = this.serviceContext.NodeContext.NodeName;

            if (this.options.IncludeMetadata && this.scope.Value != null)
            {
                eventData.EventMetadata = this.scope.Value.Metadata;
            }

            if (this.options.IncludeExceptionStackTrace && exception != null)
            {
                eventData.EventStackTrace = exception.StackTrace;
            }

            this.eventSource.WriteEvent(ref eventData);
        }

        public bool IsEnabled(
            LogLevel logLevel)
        {
            return this.options.LogLevel <= logLevel;
        }

        public IDisposable BeginScope<TState>(
            TState state)
        {
            return this.scope.Value = new Scope(this.scope, state);
        }

        private static EventLevel GetEventLevel(
            LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.None:
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return EventLevel.Verbose;
                case LogLevel.Information:
                    return EventLevel.Informational;
                case LogLevel.Warning:
                    return EventLevel.Warning;
                case LogLevel.Error:
                    return EventLevel.Error;
                case LogLevel.Critical:
                    return EventLevel.Critical;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}