using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostLoggerOptions : IServiceHostLoggerOptions
    {
        public static readonly ServiceHostLoggerOptions Disabled;

        public LogLevel LogLevel { get; set; }

        public bool IncludeMetadata { get; set; }

        public bool IncludeExceptionStackTrace { get; set; }

        static ServiceHostLoggerOptions()
        {
            Disabled = new ServiceHostLoggerOptions
            {
                LogLevel = LogLevel.None
            };
        }

        public ServiceHostLoggerOptions()
        {
            this.LogLevel = LogLevel.Information;
            this.IncludeMetadata = true;
            this.IncludeExceptionStackTrace = true;
        }
    }
}