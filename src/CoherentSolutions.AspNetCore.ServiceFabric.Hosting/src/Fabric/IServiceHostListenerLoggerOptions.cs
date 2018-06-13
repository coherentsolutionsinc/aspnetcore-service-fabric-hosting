using Microsoft.Extensions.Logging;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostListenerLoggerOptions
    {
        LogLevel LogLevel { get; }

        bool IncludeMetadata { get; }

        bool IncludeExceptionStackTrace { get; }
    }
}