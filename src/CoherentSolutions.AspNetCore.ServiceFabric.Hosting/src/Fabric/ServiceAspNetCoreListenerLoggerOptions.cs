using Microsoft.Extensions.Logging;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceAspNetCoreListenerLoggerOptions : IServiceAspNetCoreListenerLoggerOptions
    {
        public static readonly ServiceAspNetCoreListenerLoggerOptions Disabled;

        public LogLevel LogLevel { get; set; }

        public bool IncludeRequestInformation { get; set; }

        public bool IncludeExceptionStackTrace { get; set; }

        static ServiceAspNetCoreListenerLoggerOptions()
        {
            Disabled = new ServiceAspNetCoreListenerLoggerOptions
            {
                LogLevel = LogLevel.None
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceAspNetCoreListenerLoggerOptions" /> class.
        /// </summary>
        public ServiceAspNetCoreListenerLoggerOptions()
        {
            this.LogLevel = LogLevel.Information;
            this.IncludeRequestInformation = true;
            this.IncludeExceptionStackTrace = true;
        }
    }
}