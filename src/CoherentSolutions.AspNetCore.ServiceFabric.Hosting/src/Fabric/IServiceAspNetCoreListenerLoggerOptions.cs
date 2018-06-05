using Microsoft.Extensions.Logging;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceAspNetCoreListenerLoggerOptions
    {
        LogLevel LogLevel { get; }

        bool IncludeRequestInformation { get; }
    }
}