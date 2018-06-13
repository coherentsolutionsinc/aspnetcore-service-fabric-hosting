using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostListenerReplicaTemplateParameters
    {
        string EndpointName { get; }

        Func<IServiceHostListenerLoggerOptions> LoggerOptionsFunc { get; }
    }
}