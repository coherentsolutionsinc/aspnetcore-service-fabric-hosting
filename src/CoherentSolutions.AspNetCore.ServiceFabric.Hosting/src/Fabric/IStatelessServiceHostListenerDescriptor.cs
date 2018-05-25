using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatelessServiceHostListenerDescriptor
    {
        ServiceHostListenerType ListenerType { get; }

        Action<IStatelessServiceHostListenerReplicableTemplate> ConfigAction { get; }
    }
}