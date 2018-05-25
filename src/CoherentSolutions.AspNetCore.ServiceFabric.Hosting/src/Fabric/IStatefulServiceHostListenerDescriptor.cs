using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostListenerDescriptor
    {
        ServiceHostListenerType ListenerType { get; }

        Action<IStatelessServiceHostListenerReplicableTemplate> ConfigAction { get; }
    }
}