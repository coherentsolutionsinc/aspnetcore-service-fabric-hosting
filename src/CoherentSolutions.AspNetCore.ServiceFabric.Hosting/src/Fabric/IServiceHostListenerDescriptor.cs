using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostListenerDescriptor
    {
        ServiceHostListenerType ListenerType { get; }

        Action<IServiceHostListenerReplicaTemplate<IServiceHostListenerReplicaTemplateConfigurator>> ConfigAction { get; }
    }
}