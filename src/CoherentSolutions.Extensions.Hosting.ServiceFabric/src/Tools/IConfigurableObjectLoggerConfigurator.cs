using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools
{
    public interface IConfigurableObjectLoggerConfigurator
    {
        void UseLoggerOptions(
            Func<IConfigurableObjectLoggerOptions> factoryFunc);
    }
}