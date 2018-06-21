using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegateDescriptor : IServiceHostDelegateDescriptor
    {
        public Action<IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>> ConfigAction { get; }

        public ServiceHostDelegateDescriptor(
            Action<IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>> configAction)
        {
            this.ConfigAction = configAction
             ?? throw new ArgumentNullException(nameof(configAction));
        }
    }
}