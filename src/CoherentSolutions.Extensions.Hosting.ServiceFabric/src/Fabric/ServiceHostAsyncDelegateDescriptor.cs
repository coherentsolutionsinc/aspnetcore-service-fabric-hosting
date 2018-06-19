using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostAsyncDelegateDescriptor : IServiceHostAsyncDelegateDescriptor
    {
        public Action<IServiceHostAsyncDelegateReplicaTemplate<IServiceHostAsyncDelegateReplicaTemplateConfigurator>> ConfigAction { get; }

        public ServiceHostAsyncDelegateDescriptor(
            Action<IServiceHostAsyncDelegateReplicaTemplate<IServiceHostAsyncDelegateReplicaTemplateConfigurator>> configAction)
        {
            this.ConfigAction = configAction 
             ?? throw new ArgumentNullException(nameof(configAction));
        }
    }
}