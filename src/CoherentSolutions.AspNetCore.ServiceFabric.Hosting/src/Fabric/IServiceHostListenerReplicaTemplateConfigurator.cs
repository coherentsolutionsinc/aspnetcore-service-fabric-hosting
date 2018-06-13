using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostListenerReplicaTemplateConfigurator
    {
        void UseEndpointName(
            string endpointName);

        void UseLoggerOptions(
            Func<IServiceHostListenerLoggerOptions> factoryFunc);
    }
}