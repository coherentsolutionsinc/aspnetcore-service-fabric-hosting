using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public interface IConfigurableObjectLoggerOptions
    {
        LogLevel LogLevel { get; }

        bool IncludeMetadata { get; }

        bool IncludeExceptionStackTrace { get; }
    }
}