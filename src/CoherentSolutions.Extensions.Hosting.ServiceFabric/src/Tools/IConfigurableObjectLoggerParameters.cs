using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public interface IConfigurableObjectLoggerParameters
    {
        Func<IConfigurableObjectLoggerOptions> LoggerOptionsFunc { get; }
    }
}