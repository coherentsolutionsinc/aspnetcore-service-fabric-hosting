using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicaTemplateParameters
    {
        Func<IServiceEventSource> EventSource { get; }
    }
}