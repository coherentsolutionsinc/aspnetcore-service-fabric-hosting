using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostListenerDescriptor : IServiceHostListenerDescriptor
    {
        public ServiceHostListenerType ListenerType { get; }

        public Action<IServiceHostListenerReplicaTemplate<IServiceHostListenerReplicaTemplateConfigurator>> ConfigAction { get; }

        public ServiceHostListenerDescriptor(
            ServiceHostListenerType listenerType,
            Action<IServiceHostListenerReplicaTemplate<IServiceHostListenerReplicaTemplateConfigurator>> configAction)
        {
            this.ListenerType = listenerType;
            this.ConfigAction = configAction
             ?? throw new ArgumentNullException(nameof(configAction));
        }
    }
}