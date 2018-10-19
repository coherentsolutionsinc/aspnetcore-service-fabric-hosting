using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostEventSourceDescriptor : IServiceHostEventSourceDescriptor
    {
        public Action<IServiceHostEventSourceReplicaTemplate<IServiceHostEventSourceReplicaTemplateConfigurator>> ConfigAction { get; }

        public ServiceHostEventSourceDescriptor(
            Action<IServiceHostEventSourceReplicaTemplate<IServiceHostEventSourceReplicaTemplateConfigurator>> configAction)
        {
            this.ConfigAction = configAction
             ?? throw new ArgumentNullException(nameof(configAction));
        }
    }
}