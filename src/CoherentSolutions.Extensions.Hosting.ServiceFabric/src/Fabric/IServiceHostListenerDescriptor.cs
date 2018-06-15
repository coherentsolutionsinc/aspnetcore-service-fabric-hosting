using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostListenerDescriptor
    {
        ServiceHostListenerType ListenerType { get; }

        Action<IServiceHostListenerReplicaTemplate<IServiceHostListenerReplicaTemplateConfigurator>> ConfigAction { get; }
    }
}