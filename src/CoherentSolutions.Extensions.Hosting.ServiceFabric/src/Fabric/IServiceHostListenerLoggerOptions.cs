using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerLoggerOptions
    {
        LogLevel LogLevel { get; }

        bool IncludeMetadata { get; }

        bool IncludeExceptionStackTrace { get; }
    }
}