using Microsoft.Extensions.Logging;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceAspNetCoreListenerLoggerOptions : IServiceAspNetCoreListenerLoggerOptions
    {
        public LogLevel LogLevel { get; set; }

        public bool IncludeRequestInformation { get; set; }

        public ServiceAspNetCoreListenerLoggerOptions()
        {
            this.LogLevel = LogLevel.Information;
            this.IncludeRequestInformation = true;
        }
    }
}