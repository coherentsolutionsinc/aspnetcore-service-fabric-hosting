using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostLoggerOptions
    {
        LogLevel LogLevel { get; }

        bool IncludeMetadata { get; }

        bool IncludeExceptionStackTrace { get; }
    }
}