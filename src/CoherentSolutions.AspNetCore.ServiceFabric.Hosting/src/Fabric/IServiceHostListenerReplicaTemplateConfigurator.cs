using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostListenerReplicaTemplateConfigurator : IConfigurableObjectDependenciesConfigurator
    {
        void UseEndpointName(
            string endpointName);

        void UseLoggerOptions(
            Func<IServiceHostListenerLoggerOptions> factoryFunc);
    }
}