using Microsoft.Extensions.Logging;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceHostListenerLoggerOptions : IServiceHostListenerLoggerOptions
    {
        public static readonly ServiceHostListenerLoggerOptions Disabled;

        public LogLevel LogLevel { get; set; }

        public bool IncludeMetadata { get; set; }

        public bool IncludeExceptionStackTrace { get; set; }

        static ServiceHostListenerLoggerOptions()
        {
            Disabled = new ServiceHostListenerLoggerOptions
            {
                LogLevel = LogLevel.None
            };
        }

        public ServiceHostListenerLoggerOptions()
        {
            this.LogLevel = LogLevel.Information;
            this.IncludeMetadata = true;
            this.IncludeExceptionStackTrace = true;
        }
    }
}