using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerReplicaTemplateParameters : IConfigurableObjectDependenciesParameters
    {
        string EndpointName { get; }

        Func<IServiceHostLoggerOptions> LoggerOptionsFunc { get; }
    }
}