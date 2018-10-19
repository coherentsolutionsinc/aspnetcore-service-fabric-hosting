using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostLoggerConfigurator
    {
        void UseLoggerOptions(
            Func<IServiceHostLoggerOptions> factoryFunc);
    }
}