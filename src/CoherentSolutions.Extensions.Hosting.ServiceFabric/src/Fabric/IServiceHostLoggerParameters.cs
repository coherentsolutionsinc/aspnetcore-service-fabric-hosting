using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostLoggerParameters
    {
        Func<IServiceHostLoggerOptions> LoggerOptionsFunc { get; }
    }
}