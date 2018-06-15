using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicaTemplateConfigurator : IConfigurableObjectDependenciesConfigurator
    {
        void UseEndpointName(
            string endpointName);

        void UseLoggerOptions(
            Func<IServiceHostListenerLoggerOptions> factoryFunc);
    }
}