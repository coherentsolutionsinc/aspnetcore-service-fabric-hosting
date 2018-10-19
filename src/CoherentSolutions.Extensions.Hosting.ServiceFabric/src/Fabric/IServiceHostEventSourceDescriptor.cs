using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceDescriptor
    {
        Action<IServiceHostEventSourceReplicaTemplate<IServiceHostEventSourceReplicaTemplateConfigurator>> ConfigAction { get; }
    }
}