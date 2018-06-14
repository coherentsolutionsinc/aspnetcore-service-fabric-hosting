using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostListenerReplicaTemplateParameters : IConfigurableObjectDependenciesParameters
    {
        string EndpointName { get; }

        Func<IServiceHostListenerLoggerOptions> LoggerOptionsFunc { get; }
    }
}